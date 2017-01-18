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
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations.Support {

    public class AppendToRowOperation : AbstractOperation {

        public AppendToRowOperation() {
            UseTransaction = false;
        }

        private readonly IDictionary<string, object> _data;

        public AppendToRowOperation(string key, object value) {
            _data = new Dictionary<string, object> { { key, value } };
        }

        public AppendToRowOperation(IDictionary<string, object> data) {
            _data = data;
        }

        public AppendToRowOperation Add(KeyValuePair<string, object> data) {
            _data.Add(data);
            return this;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var kv in _data) {
                    row.Add(kv.Key, kv.Value);
                }
                yield return row;
            }
        }
    }
}
