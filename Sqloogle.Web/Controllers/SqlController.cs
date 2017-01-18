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
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Sqloogle.Search;
using Sqloogle.Web.Models;

namespace Sqloogle.Web.Controllers {

    [SessionState(SessionStateBehavior.Disabled)]
    public class SqlController : Controller {

        private const string ContentPlain = "text/plain";
        private const string ContentJavascript = "text/javascript";

        // GET: /Sql/
        public string Index() {
            var response = new { success = false, message = "Invalid Operation.  Please use Search, Find, or Download." };
            var callback = Request.QueryString.AllKeys.Any(k => k == "callback") ? Request.QueryString.Get("callback") : string.Empty;

            Response.ContentType = string.IsNullOrEmpty(callback) ? ContentPlain : ContentJavascript;

            return
                string.IsNullOrEmpty(callback)
                    ? System.Web.Helpers.Json.Encode(response)
                    : $"{callback}({System.Web.Helpers.Json.Encode(response)});";
        }

        // GET: /Sql/Search?
        public string Search(string q, string callback) {
            Response.ContentType = string.IsNullOrEmpty(callback) ? ContentPlain : ContentJavascript;

            var searchResponse = new SearchResponse();

            if (!string.IsNullOrEmpty(q)) {
                try {
                    var searcher = new SqloogleSearcher(ConfigurationManager.AppSettings.Get("SearchIndexPath"));
                    var results = searcher.Search(q);
                    foreach (var result in results)
                        searchResponse.searchresults.Add(new Models.ScriptedObjects.SearchResult(result, this));
                } catch (Exception e) {
                    searchResponse.success = false;
                    searchResponse.message = e.Message;
                }

            }

            return
                string.IsNullOrEmpty(callback) ?
                searchResponse.ToJson() :
                    $"{callback}({searchResponse.ToJson()});";
        }

        // GET: /Sql/Find?
        public string Find(string id, string callback) {
            Response.ContentType = String.IsNullOrEmpty(callback) ? ContentPlain : ContentJavascript;

            var result = !string.IsNullOrEmpty(id) ? new SqloogleSearcher(ConfigurationManager.AppSettings.Get("SearchIndexPath")).Find(id) : null;

            if (result != null) {
                var searchResponse = new SearchResponse();
                searchResponse.searchresults.Add(new Models.ScriptedObjects.SearchResult(result, this));

                return
                    string.IsNullOrEmpty(callback) ?
                    searchResponse.ToJson() :
                        $"{callback}({searchResponse.ToJson()});";
            }

            throw new HttpException(404, "NotFound");
        }

        // GET: /Sql/Download?
        public ActionResult Download(string id) {
            var result = !string.IsNullOrEmpty(id) ? new SqloogleSearcher(ConfigurationManager.AppSettings.Get("SearchIndexPath")).Find(id) : null;
            if (result != null) {
                var cd = new System.Net.Mime.ContentDisposition {
                    FileName = $"{id}.sql",
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(new MemoryStream(Encoding.ASCII.GetBytes(result["sqlscript"])), "application/x-sql");
            }
            throw new HttpException(404, "NotFound");
        }

    }
}
