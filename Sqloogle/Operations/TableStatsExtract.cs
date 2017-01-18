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
using System.Data;
using Rhino.Etl.Core;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {
    public class TableStatsExtract : InputOperation {

        public TableStatsExtract(string connectionString)
            : base(connectionString)
        {
            UseTransaction = false;
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
