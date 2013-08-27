using System.Collections.Generic;
using System.Web.Helpers;

namespace WebService.Models.ScriptedObjects
{
    public class SearchResponse
    {
        public List<SearchResult> searchresults = new List<SearchResult>();
        public bool success = true;
        public string message = string.Empty;

        public string ToJson()
        {
            return Json.Encode(this);
        }
    }
}