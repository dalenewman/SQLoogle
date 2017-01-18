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

    public class CachedSqlAggregate : AbstractAggregationOperation {

        public CachedSqlAggregate() {
            UseTransaction = false;
        }

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
