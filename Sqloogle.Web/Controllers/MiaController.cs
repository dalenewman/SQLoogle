#region license
// Sqloogle
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Sqloogle.Search;
using Sqloogle.Web.Models.MissingIndices;
using SearchResponse = Sqloogle.Web.Models.SearchResponse;

namespace Sqloogle.Web.Controllers {

    [SessionState(SessionStateBehavior.Disabled)]
    public class MiaController : Controller {
        private readonly SqloogleMiaSearcher _searcher;

        public MiaController() {
            var indexPath = Path.Combine(ConfigurationManager.AppSettings.Get("SearchIndexPath"), "MIA");
            _searcher = new SqloogleMiaSearcher(indexPath);
        }


        // GET: /Mia/
        public string Index() {

            var response = new { success = false, message = "Invalid Operation.  Please use Search." };
            var callback = Request.QueryString.AllKeys.Any(k => k == "callback") ? Request.QueryString.Get("callback") : string.Empty;

            Response.ContentType = string.IsNullOrEmpty(callback) ? "text/plain" : "text/javascript";

            return
                string.IsNullOrEmpty(callback)
                    ? System.Web.Helpers.Json.Encode(response)
                    : $"{callback}({System.Web.Helpers.Json.Encode(response)});";
        }

        //
        // GET: /Mia/Search?
        public string Search(string q, string callback) {
            Response.ContentType = string.IsNullOrEmpty(callback) ? "text/plain" : "text/javascript";

            var searchResponse = new SearchResponse();

            if (string.IsNullOrEmpty(q)) {
                int limit;
                int.TryParse(ConfigurationManager.AppSettings.Get("SearchResultsLimit"), out limit);
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
                string.IsNullOrEmpty(callback) ?
                searchResponse.ToJson() :
                    $"{callback}({searchResponse.ToJson()});";
        }

    }
}
