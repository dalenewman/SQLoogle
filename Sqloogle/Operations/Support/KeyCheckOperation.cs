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

    public class KeyCheckOperation : AbstractOperation {

        public KeyCheckOperation() {
            UseTransaction = false;
        }

        private readonly List<string> _keys = new List<string>();

        public KeyCheckOperation(IEnumerable<string> keys) {
            _keys.AddRange(keys);
        }

        public KeyCheckOperation AddRange(IEnumerable<string> keys) {
            _keys.AddRange(keys);
            return this;
        }

        public KeyCheckOperation AddParams(params string[] keys) {
            _keys.AddRange(keys);
            return this;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var key in _keys) {
                    Guard.Against(!row.Contains(key), string.Format("Row must contain {0} key", key));
                }
                yield return row;
            }
        }
    }
}
