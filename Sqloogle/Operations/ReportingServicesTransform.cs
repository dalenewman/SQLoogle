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
using System.IO;
using System.Xml.Linq;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {

    public class ReportingServicesTransform : AbstractOperation {

        public ReportingServicesTransform() {
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {

                var rdl = XDocument.Parse(row["rdl"].ToString());

                if (rdl.Root == null)
                    continue;

                var nameSpace = rdl.Root.GetDefaultNamespace().NamespaceName;
                var commands = rdl.Root.Descendants("{" + nameSpace + "}CommandText");
                var counter = 0;

                foreach (var command in commands) {
                    counter++;
                    var commandRow = new Row();
                    commandRow.Copy(row);

                    commandRow["sqlscript"] = command.Value;
                    commandRow["name"] = row["name"] + " - " + counter.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
                    commandRow["path"] = Path.Combine("Reporting Services", row["path"].ToString().Replace("/", "\\").TrimStart('\\'));
                    commandRow["type"] = "SSRS Command";
                    commandRow["schema"] = string.Empty;
                    commandRow["lastused"] = DateTime.MinValue;
                    yield return commandRow;
                }

            }
        }
    }

}
