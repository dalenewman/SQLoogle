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
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Sqloogle.Utilities;

namespace Sqloogle.Web.Models.ScriptedObjects {

    public class SearchResult {

        public string Id { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public float Rank { get; set; }
        public bool Dropped { get; set; }
        public long Use { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateFormatted { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifyDateFormatted { get; set; }
        public DateTime LastUsedDate { get; set; }
        public string LastUsedDateFormatted { get; set; }
        public string SqlScript { get; set; }

        public SearchResult(IDictionary<string, string> dict, Controller controller) {
            Name = dict["name"];
            Database = dict["database"];
            Id = dict["id"];
            Schema = dict["schema"];
            Server = dict["server"];
            Type = dict["type"];
            Url = controller.Url.Content($"~/Sql/Download?id={HttpUtility.UrlEncode(dict["id"])}");
            CreateDate = Dates.ConvertDocDate(dict["created"]);
            CreateDateFormatted = Dates.FormatDate(Dates.ConvertDocDate(dict["created"]));
            ModifyDate = Dates.ConvertDocDate(dict["modified"]);
            ModifyDateFormatted = Dates.FormatDate(Dates.ConvertDocDate(dict["modified"]));
            Rank = float.Parse(dict["rank"]);
            Dropped = Convert.ToBoolean(dict["dropped"]);
            Use = Convert.ToInt64(dict["use"]);
            LastUsedDate = Dates.ConvertDocDate(dict["lastused"]);
            LastUsedDateFormatted = Dates.FormatDate(Dates.ConvertDocDate(dict["lastused"]));
            SqlScript = dict["sqlscript"];
        }
    }
}