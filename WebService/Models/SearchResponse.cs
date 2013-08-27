using System.Collections.Generic;
using System.Web.Helpers;

namespace WebService.Models
{
    public class SearchResponse
    {
        public List<object> searchresults = new List<object>();
        public bool success = true;
        public string message = string.Empty;

        public string ToJson()
        {
            return Json.Encode(this);
        }
    }
}