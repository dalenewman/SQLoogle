using System.Collections.Generic;
using System.Web.Helpers;

namespace WebService.Models.MissingIndices
{
    public class SearchResponse
    {
        public List<Models.MissingIndices.SearchResult> searchresults = new List<Models.MissingIndices.SearchResult>();
        public bool success = true;
        public string message = string.Empty;

        public string ToJson()
        {
            return Json.Encode(this);
        }
    }
}