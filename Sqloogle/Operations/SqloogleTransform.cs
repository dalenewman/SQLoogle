using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Utilities;
using System.Linq;

namespace Sqloogle.Operations {

    public class SqloogleTransform : AbstractOperation {

        private const string DATE_FORMAT = "yyyyMMdd";
        private const RegexOptions OPTIONS = RegexOptions.Compiled;

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                var sql = SqlTransform(row["sqlscript"]);

                row["sql"] = sql;
                row["id"] = sql.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "X");
                row["created"] = DateTransform(row["created"], DateTime.Today);
                row["modified"] = DateTransform(row["modified"], DateTime.Today);
                row["lastused"] = DateTransform(row["lastused"], DateTime.MinValue);
                row["server"] = ListTransform(row["server"]);
                row["database"] = ListTransform(row["database"]);
                row["schema"] = ListTransform(row["schema"]);
                row["name"] = ListTransform(row["name"]);
                row["use"] = Strings.UseBucket(row["use"]);
                row["count"] = row["count"].ToString().PadLeft(10, '0');
                row["dropped"] = false;
                yield return row;
            }
        }

        private static string ListTransform(object list) {
            if (list == null)
                return string.Empty;

            var items = (HashSet<string>)list;

            var strings = (from item in items where !string.IsNullOrEmpty(item) select item).ToArray();
            var result = strings.Length > 0 ? string.Join(" | ", strings.OrderBy(s => s)) : string.Empty;
            return result.Equals("System.Object[]") ? string.Empty : result;
        }

        private static string SqlTransform(object sql) {
            const string space = " ";
            var clean = Strings.SplitTitleCase(SqlStrings.RemoveSqlPunctuation(sql), space).ToLower();
            return sql.ToString().ToLower() + " " + string.Join(space, clean.Split(space.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Distinct());
        }

        private static string DateTransform(object possibleDate, DateTime defaultDate) {
            if (possibleDate == null) {
                return defaultDate.ToString(DATE_FORMAT);
            }

            var actualDate = Convert.ToDateTime(possibleDate);
            if (actualDate == DateTime.MinValue && defaultDate != DateTime.MinValue)
                return defaultDate.ToString(DATE_FORMAT);

            return actualDate.ToString(DATE_FORMAT);
        }

        public string FilePath(string outputFolder, Row row) {
            if ((int)row["count"] > 1)
                throw new Exception("You have to write files before you group.");

            var name = Regex.Replace(Regex.Replace(row["name"].ToString(), @"[^\w-]", " ", OPTIONS), @"^\s+|\s+|\s+$", " ", OPTIONS).Trim(' ');
            var path = row["path"].ToString().TrimStart('\\');
            var schema = row["schema"].ToString() == string.Empty ? "dbo" : row["schema"].ToString();

            return Path.Combine(outputFolder, row["server"].ToString(), row["database"].ToString(), schema, path, name) + ".sql";
        }

    }
}
