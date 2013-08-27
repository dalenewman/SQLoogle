﻿using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {
    public class TableStatsJoin : JoinOperation {

        protected override Row MergeRows(Row leftRow, Row rightRow) {

            var row = leftRow.Clone();

            if (!leftRow["type"].Equals("Table"))
                return row;

            if(rightRow["use"] != null)
                row["use"] = rightRow["use"];

            if(rightRow["lastused"] != null)
                row["lastused"] = rightRow["lastused"];

            return row;
        }

        protected override void SetupJoinConditions() {
            LeftJoin
                .Left("database", "objectid")
                .Right("database", "objectid");
        }
    }
}
