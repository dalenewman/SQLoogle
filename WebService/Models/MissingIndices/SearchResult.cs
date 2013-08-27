using System;
using System.Collections.Generic;
using Sqloogle.Utilities;

namespace WebService.Models.MissingIndices {
    public class SearchResult {
        public string Id { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }
        public string Equality { get; set; }
        public string Inequality { get; set; }
        public string Included { get; set; }
        public long Score { get; set; }
        public float Rank { get; set; }
        public DateTime LastNeededDate { get; set; }
        public string LastNeededDateFormatted { get; set; }
        public string SqlQuery { get; set; }
        public string CreateIndexSql { get; set; }
        public string SqlIndexQuery { get; set; }

        public SearchResult(IDictionary<string, string> dict) {
            Id = dict["id"];
            Name = dict["name"];
            Database = dict["database"];
            Schema = dict["schema"];
            Server = dict["server"];
            Score = (long) Math.Round(Convert.ToDecimal(dict["score"]), 0);
            Rank = float.Parse(dict["rank"]);
            Equality = dict["equality"];
            Included = dict["included"];
            Inequality = dict["inequality"];
            LastNeededDate = Dates.ConvertDocDate(dict["lastneeded"]);
            LastNeededDateFormatted = Dates.FormatDate(LastNeededDate);
            SqlQuery = CreateExpressionToSearchForPossibleCauses();
            SqlIndexQuery = CreateExpressionToSearchForOtherIndexes();
            CreateIndexSql = CreateMissingIndexSql();
        }

        public string ColumnSummary {
            get {
                var columns = new List<string>();
                if (!string.IsNullOrEmpty(Equality))
                    columns.Add("<span>equality:</span> " + Strings.RemoveBrackets(Equality));
                if (!string.IsNullOrEmpty(Inequality))
                    columns.Add("<span>inequality:</span> " + Strings.RemoveBrackets(Inequality));
                if (!string.IsNullOrEmpty(Included))
                    columns.Add("<span>included:</span> " + Strings.RemoveBrackets(Included));
                return string.Join("<br/>", columns);
            }
        }

        public String CreateExpressionToSearchForPossibleCauses() {
            var columns = Strings.RemoveBracketsAndCommas(string.Concat(Equality, " ", Inequality, " ", Included));
            return string.Format("server:{0} {1} -type:table -type:index {2}", Server, Name, columns);
        }

        public String CreateExpressionToSearchForOtherIndexes() {
            return string.Format("server:{0} database:{1} schema:{2} sql:\"{3}\" type:(table OR index)", Server, Database, Schema, Name);
        }

        private String CreateIndexName() {
            const string indexNameTemplate = "IX_{0}_{1}__{2}";
            var indexColumns = Strings.RemoveBracketsAndCommas(string.Concat(Equality, "_", Inequality));
            var includedColumns = Strings.RemoveBracketsAndCommas(Included);
            var indexName = string.Format(indexNameTemplate, Name, indexColumns, includedColumns).Replace(" ", "_").TrimEnd("_".ToCharArray());
            if (indexName.Length > 128)
                indexName = indexName.Substring(0, 128);
            return indexName;
        }

        private String CreateIncludedColumnsClause() {
            const string includedColumnsTemplate = "\n\t\tINCLUDE({0})";
            return String.IsNullOrEmpty(Included) ? string.Empty : string.Format(includedColumnsTemplate, Included);
        }

        public String CreateMissingIndexSql() {
            const string sqlTemplate = "CREATE NONCLUSTERED INDEX [{0}]\n\tON [{1}].[{2}]({3}){4};";
            var indexColumns = String.Concat(Equality, String.IsNullOrEmpty(Inequality) ? string.Empty : ", " + Inequality);
            return string.Format(sqlTemplate, CreateIndexName(), Schema, Name, indexColumns.TrimStart(", ".ToCharArray()), CreateIncludedColumnsClause());
        }

    }
}
