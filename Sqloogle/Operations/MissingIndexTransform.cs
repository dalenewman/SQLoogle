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
using System.Globalization;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;
using Sqloogle.Utilities;

namespace Sqloogle.Operations {
    public class MissingIndexTransform : AbstractOperation {

        private const string DATE_FORMAT = "yyyyMMdd";

        public MissingIndexTransform() {
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var key = string.Format("{0}{1}{2}{3}{4}{5}{6}", row["server"], row["database"], row["schema"], row["name"], row["equality"], row["inequality"], row["included"]);
                row["id"] = key.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "X");
                row["lastneeded"] = DateTransform(row["lastneeded"], DateTime.MinValue);
                row["action"] = "Create";
                row["score"] = Math.Round(Convert.ToDouble(row["score"])).ToString().PadLeft(10, '0');
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
