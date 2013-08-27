﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {

    public class ReportingServicesTransform : AbstractOperation {

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
