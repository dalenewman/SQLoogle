using System;
using System.Collections.Generic;
using System.Linq;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {

    public class SqloogleAggregate : AbstractAggregationOperation {

        protected override void Accumulate(Row row, Row aggregate) {

            //take first
            foreach (var field in new[] { "sqlscript", "type" }) {
                WarnIfNull(row, field);
                if (aggregate[field] == null) {
                    aggregate[field] = row[field];
                }
            }

            //take max
            foreach (var field in new[] { "created", "modified", "lastused" }) {
                WarnIfNull(row, field);
                aggregate[field] = new[] { aggregate[field], row[field] }.Max();
            }

            //combine
            foreach (var field in new[] { "server", "database", "schema", "name" }) {
                WarnIfNull(row, field);

                if (aggregate[field] == null) {
                    aggregate[field] = new HashSet<string> {row[field].ToString()};
                } else {
                    var existing = (HashSet<string>)aggregate[field];
                    if (existing.Contains(row[field])) continue;

                    existing.Add(row[field].ToString());
                    aggregate[field] = existing;
                }
            }

            //add use
            if (aggregate["use"] == null) { aggregate["use"] = (long)0; }
            if (row["use"] != null)
                aggregate["use"] = (Convert.ToInt64(aggregate["use"]) + Convert.ToInt64(row["use"]));

            // count
            if (aggregate["count"] == null) { aggregate["count"] = 0; }
            aggregate["count"] = ((int)aggregate["count"]) + 1;

        }

        private void WarnIfNull(QuackingDictionary row, string field) {
            if (row[field] == null) {
                Warn("Null in {0}! Type: {1}", field, row["type"]);
            }
        }

        protected override string[] GetColumnsToGroupBy() {
            return new[] { "sqlscript" };
        }

    }
}
