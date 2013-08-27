using System;
using System.Web.Mvc;
using Sqloogle.MissingIndexes;
using Sqloogle.Utilities;
using WebService.Models.MissingIndices;
using System.Linq;

namespace WebService.Controllers
{
    public class MissingIndexController : Controller
    {
        private readonly MissingIndexRepository _repository = new MissingIndexRepository(Enums.DirectoryType.FileSystem);

        // GET: /MissingIndex/Search?
        public string Search(string q, string callback)
        {
            Response.ContentType = String.IsNullOrEmpty(callback) ? "text/plain" : "text/javascript";

            var searchResponse = new SearchResponse();

            if (string.IsNullOrEmpty(q))
            {
                searchResponse.searchresults.AddRange(_repository.ReadHighScores().Select(mi=> new SearchResult(mi)));
            } else {
                try
                {
                    var searcher = new MissingIndexSearcher();
                    var missingIndices = searcher.Search(q);
                    foreach (var mi in missingIndices)
                        searchResponse.searchresults.Add(new SearchResult(mi));
                }
                catch (Exception e)
                {
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
