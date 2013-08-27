using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sqloogle.Search;
using WebService.Models;
using WebService.Models.ScriptedObjects;

namespace WebService.Controllers {
    public class SqlController : Controller {

        private const string CONTENT_PLAIN = "text/plain";
        private const string CONTENT_JAVASCRIPT = "text/javascript";

        // GET: /Sql/
        public string Index() {
            var response = new { success = false, message = "Invalid Operation.  Please use Search, Find, or Download." };
            var callback = Request.QueryString.AllKeys.Any(k => k == "callback") ? Request.QueryString.Get("callback") : string.Empty;

            Response.ContentType = String.IsNullOrEmpty(callback) ? CONTENT_PLAIN : CONTENT_JAVASCRIPT;

            return
                string.IsNullOrEmpty(callback)
                    ? System.Web.Helpers.Json.Encode(response)
                    : string.Format("{0}({1});", callback, System.Web.Helpers.Json.Encode(response));
        }

        // GET: /Sql/Search?
        public string Search(string q, string callback) {
            Response.ContentType = String.IsNullOrEmpty(callback) ? CONTENT_PLAIN : CONTENT_JAVASCRIPT;

            var searchResponse = new SearchResponse();

            if (!string.IsNullOrEmpty(q)) {
                try {
                    var searcher = new SqloogleSearcher(Config.SearchIndexPath);
                    var results = searcher.Search(q);
                    foreach (var result in results)
                        searchResponse.searchresults.Add(new SearchResult(result, this));
                } catch (Exception e) {
                    searchResponse.success = false;
                    searchResponse.message = e.Message;
                }

            }

            return
                String.IsNullOrEmpty(callback) ?
                searchResponse.ToJson() :
                string.Format("{0}({1});", callback, searchResponse.ToJson());
        }

        // GET: /Sql/Find?
        public string Find(string id, string callback) {
            Response.ContentType = String.IsNullOrEmpty(callback) ? CONTENT_PLAIN : CONTENT_JAVASCRIPT;

            var result = !string.IsNullOrEmpty(id) ? new SqloogleSearcher(Config.SearchIndexPath).Find(id) : null;

            if (result != null)
            {
                var searchResponse = new SearchResponse();
                searchResponse.searchresults.Add(new SearchResult(result, this));

                return
                    String.IsNullOrEmpty(callback) ?
                    searchResponse.ToJson() :
                    string.Format("{0}({1});", callback, searchResponse.ToJson());
            }

            throw new HttpException(404, "NotFound");
        }

        // GET: /Sql/Download?
        public ActionResult Download(string id) {
            var result = !string.IsNullOrEmpty(id) ? new SqloogleSearcher(Config.SearchIndexPath).Find(id) : null;
            if (result != null) {
                var cd = new System.Net.Mime.ContentDisposition {
                    FileName = string.Format("{0}.sql", id),
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(new MemoryStream(Encoding.ASCII.GetBytes(result["sqlscript"])), "application/x-sql");
            }
            throw new HttpException(404, "NotFound");
        }

    }
}
