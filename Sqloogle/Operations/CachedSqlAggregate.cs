using System;
using System.Collections.Generic;
using System.Linq;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {

    public class CachedSqlAggregate : AbstractAggregationOperation {

        protected override string[] GetColumnsToGroupBy() {
            return new[] { "sqlscript" };
        }

        protected override void Accumulate(Row row, Row aggregate) {

            // init
            if (aggregate["sqlscript"] == null)
                aggregate["sqlscript"] = row["sqlscript"];

            if (aggregate["type"] == null)
                aggregate["type"] = row["type"];

            if (aggregate["database"] == null) {
                aggregate["database"] = new Object[0];
            }

            if (aggregate["use"] == null) {
                aggregate["use"] = 0;
            }

            //aggregate
            if (row["database"] != null) {
                var existing = new List<Object>((Object[])aggregate["database"]);
                if (!existing.Contains(row["database"])) {
                    existing.Add(row["database"]);
                    aggregate["database"] = existing.ToArray();
                }
            }

            aggregate["use"] = ((int)aggregate["use"]) + ((int)row["use"]);
        }
    }
}
