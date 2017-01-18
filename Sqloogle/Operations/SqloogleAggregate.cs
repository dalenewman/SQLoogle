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
using System.Linq;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {

    public class SqloogleAggregate : AbstractAggregationOperation {

        public SqloogleAggregate() {
            UseTransaction = false;
        }

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
                    aggregate[field] = new HashSet<string> { row[field].ToString() };
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
