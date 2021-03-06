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
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {
    public class CachedObjectStatsJoin : JoinOperation {

        public CachedObjectStatsJoin() {
            UseTransaction = false;
        }

        protected override Row MergeRows(Row leftRow, Row rightRow) {

            var row = leftRow.Clone();
            if (rightRow["use"] != null) {
                row["use"] = rightRow["use"];
                row["lastused"] = DateTime.Now;
            }
            return row;
        }

        protected override void SetupJoinConditions() {
            LeftJoin.Left("database", "objectid").Right("database", "objectid");
        }
    }
}