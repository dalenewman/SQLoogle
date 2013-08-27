using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Sqloogle.Utilities;

namespace WebService.Models.ScriptedObjects {

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
            Url = controller.Url.Content(string.Format("~/Sql/Download?id={0}", HttpUtility.UrlEncode(dict["id"])));
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