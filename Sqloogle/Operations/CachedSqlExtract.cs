﻿using System.Data;
using Rhino.Etl.Core;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {
    public class CachedSqlExtract : InputOperation {

        public CachedSqlExtract(string connectionString)
            : base(connectionString) {
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = @"/* SQLoogle */
                SELECT
	                ph.[text] AS sqlscript
	                ,DB_NAME(ph.[dbid]) AS [database]
                    ,MAX(cp.objtype) AS [type]
                    ,MAX(cp.usecounts) AS [use]
                FROM sys.dm_exec_cached_plans cp WITH (NOLOCK)
                CROSS APPLY sys.dm_exec_sql_text(plan_handle) ph
                WHERE cp.cacheobjtype = N'Compiled Plan' 
                AND cp.objtype IN (N'Adhoc', N'Prepared')
                AND ph.[text] NOT LIKE '/* SQLoogle */%'
                GROUP BY ph.[text], DB_NAME(ph.[dbid])
                ORDER BY ph.[text];
            ";
        }
    }
}
