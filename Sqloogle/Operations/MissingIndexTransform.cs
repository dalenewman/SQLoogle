using System;
using System.Collections.Generic;
using System.Globalization;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Utilities;

namespace Sqloogle.Operations {
    public class MissingIndexTransform : AbstractOperation {

        private const string DATE_FORMAT = "yyyyMMdd";

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var key = string.Format("{0}{1}{2}{3}{4}{5}{6}", row["server"], row["database"], row["schema"], row["name"], row["equality"], row["inequality"], row["included"]);
                row["id"] = key.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "X");
                row["lastneeded"] = DateTransform(row["lastneeded"], DateTime.MinValue);
                row["action"] = "Create";
                row["score"] = Math.Round(Convert.ToDouble(row["score"])).ToString().PadLeft(10,'0');
                row.Remove("connectionstring");
                row.Remove("compatibilitylevel");
                row.Remove("databaseid");
                yield return row;
            }
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

    }
}
