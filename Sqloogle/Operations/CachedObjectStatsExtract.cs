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

    public class CachedObjectStatsExtract : InputOperation {

        public CachedObjectStatsExtract(string connectionString)
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
                    DB_NAME(ph.dbid) AS [database]
	                ,ph.objectid AS objectid
                    ,MAX(cp.usecounts) AS [use]
                FROM master.sys.dm_exec_cached_plans cp WITH (NOLOCK)
                CROSS APPLY master.sys.dm_exec_sql_text(plan_handle) ph
                WHERE cp.cacheobjtype != 'Extended Proc'
                AND ph.objectid IS NOT NULL
                AND DB_NAME(ph.dbid) IS NOT NULL
                GROUP BY 
                    DB_NAME(ph.dbid),
                    ph.objectid
                ORDER BY [Database], ObjectId
            ";
        }
    }
}
