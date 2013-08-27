﻿using System.Data;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {
    public class TableStatsExtract : InputOperation {

        public TableStatsExtract(string connectionString)
            : base(connectionString) {
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = @"/* SQLoogle */

                SELECT
                    DB_NAME(ius.database_id) AS [database]
	                ,ius.[object_id] AS objectid
                    ,SUM(user_seeks + user_scans + user_lookups + user_updates) AS [use]
                    ,MAX(COALESCE(last_user_seek, last_user_scan, last_user_lookup, last_user_update)) AS lastused
                FROM sys.dm_db_index_usage_stats ius WITH (NOLOCK)
                WHERE (user_seeks + user_scans + user_lookups + user_updates) > 0
                GROUP BY
					DB_NAME(ius.database_id)
	                ,ius.[object_id]
	            ORDER BY [Database], ObjectId
            ";
        }
    }
}
