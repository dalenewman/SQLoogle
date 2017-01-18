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
using System.Collections.Generic;
using System.Linq;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations.Support {

    public class UnionOperation : AbstractAggregationOperation {

        private readonly IEnumerable<string> _groupByColumns;

        public UnionOperation(IEnumerable<string> groupByColumns)
        {
            UseTransaction = false;
            _groupByColumns = groupByColumns;
        }

        protected override void Accumulate(Row row, Row aggregate) {

            foreach (var column in _groupByColumns) {
                aggregate[column] = row[column];
            }

            if (aggregate["Count"] == null)
                aggregate["Count"] = 0;

            aggregate["Count"] = (int)aggregate["Count"] + 1;
        }

        protected override string[] GetColumnsToGroupBy() {
            return _groupByColumns.ToArray();
        }
    }
}
