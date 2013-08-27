using System;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {
    public class CachedObjectStatsJoin : JoinOperation {

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