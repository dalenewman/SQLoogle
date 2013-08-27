using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Sqloogle.Search;
using WebService.Models;
using WebService.Models.MissingIndices;

namespace WebService.Controllers {
    public class MiaController : Controller {
        private readonly SqloogleMiaSearcher _searcher;

        public MiaController() {
            var indexPath = Path.Combine(Config.SearchIndexPath, "MIA");
            _searcher = new SqloogleMiaSearcher(indexPath);
        }


        // GET: /Mia/
        public string Index() {

            var response = new { success = false, message = "Invalid Operation.  Please use Search." };
            var callback = Request.QueryString.AllKeys.Any(k => k == "callback") ? Request.QueryString.Get("callback") : string.Empty;

            Response.ContentType = String.IsNullOrEmpty(callback) ? "text/plain" : "text/javascript";

            return
                string.IsNullOrEmpty(callback)
                    ? System.Web.Helpers.Json.Encode(response)
                    : string.Format("{0}({1});", callback, System.Web.Helpers.Json.Encode(response));
        }

        //
        // GET: /Mia/Search?
        public string Search(string q, string callback) {
            Response.ContentType = String.IsNullOrEmpty(callback) ? "text/plain" : "text/javascript";

            var searchResponse = new SearchResponse();

            if (string.IsNullOrEmpty(q)) {
                int limit;
                int.TryParse(Config.SearchResultsLimit, out limit);
                searchResponse.searchresults.AddRange(_searcher.ReadHighScores(limit).Select(mi => new SearchResult(mi)));
            } else {
                try {
                    var missingIndices = _searcher.Search(q);
                    foreach (var mi in missingIndices)
                        searchResponse.searchresults.Add(new SearchResult(mi));
                }
                catch (Exception e) {
                    searchResponse.success = false;
                    searchResponse.message = e.Message;
                }

            }

            return
                String.IsNullOrEmpty(callback) ?
                searchResponse.ToJson() :
                string.Format("{0}({1});", callback, searchResponse.ToJson());
        }

    }
}
