﻿#region license
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
using System.Linq;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {

    /// <summary>
    /// SqloogleCompare has the aweful job of determining what we should
    /// do with each freshly crawled and existing sql objects.  We'll end up
    /// with either a create, an update to dropped status, a None (no action)
    /// or a property update.
    /// </summary>
    public class SqloogleCompare : JoinOperation {

        private readonly string _today;

        public SqloogleCompare()
        {
            UseTransaction = false;
            _today = DateTime.Today.ToString("yyyyMMdd");
        }

        protected override Row MergeRows(Row newRow, Row oldRow) {

            Row row;

            // if the old row doesn't exist, then the new row is new, and it should be created
            if (oldRow["id"] == null) {
                row = newRow.Clone();
                row["action"] = "Create";
                return row;
            }

            // if the new row doesn't exist, then the old row has been dropped, and it should be marked as dropped and updated
            if (newRow["id"] == null) {
                row = oldRow.Clone();
                row["dropped"] = true;
                row["action"] = oldRow["dropped"].Equals(true) ? "None" : "Update";
                return row;
            }

            // if new and old rows are the same, then we have nothing to do.
            if (Equal(newRow, oldRow)) {
                row = oldRow.Clone();
                row["action"] = "None";
                return row;
            }

            // if we end up here, the sql is the same but other properties have been updated, so we should update it.
            row = newRow.Clone();
            row["action"] = "Update";
            row["modified"] = _today;
            return row;
        }

        private static bool Equal(QuackingDictionary newRow, QuackingDictionary oldRow) {
            var fields = new string[] { "use", "lastused", "count", "created", "name", "server", "database", "schema", "dropped" };
            return fields.All(field => newRow[field].Equals(oldRow[field]));
        }

        protected override void SetupJoinConditions() {
            FullOuterJoin.Left("id").Right("id");
        }
    }
}
