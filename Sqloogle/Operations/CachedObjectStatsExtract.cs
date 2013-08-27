using System.Data;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {

    public class CachedObjectStatsExtract : InputOperation {

        public CachedObjectStatsExtract(string connectionString)
            : base(connectionString) {
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
