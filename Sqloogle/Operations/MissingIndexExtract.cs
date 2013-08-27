using System.Collections.Generic;
using System.Data;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {

    public class MissingIndexExtract : AbstractOperation {

        private readonly string _server;

        public MissingIndexExtract(string server) {
            _server = server;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                var connectionString = row["connectionstring"].ToString();
                var sqlConnectionChecker = new SqlConnectionChecker(new[] { connectionString });

                if (!sqlConnectionChecker.AllGood())
                    continue;

                foreach (var subRow in new InternalOperation(connectionString).Execute(null)) {
                    subRow.Add("server", _server);
                    foreach (var column in row.Columns) {
                        subRow.Add(column, row[column]);
                    }
                    yield return subRow;
                }
            }
        }

        private class InternalOperation : InputOperation {

            public InternalOperation(string connectionString)
                : base(connectionString) {
            }

            protected override Row CreateRowFromReader(IDataReader reader) {
                return Row.FromReader(reader);
            }

            protected override void PrepareCommand(IDbCommand cmd) {
                cmd.CommandText = @"/* SQLoogle */

                    SELECT
	                    s.name AS [schema],
	                    o.name AS [name],
	                    p.rows,
	                    p.sizemb AS [size],
	                    ISNULL(id.equality_columns, '') AS equality,
	                    ISNULL(id.inequality_columns, '') AS inequality,
	                    ISNULL(id.included_columns, '') AS included,
	                    igs.unique_compiles AS compiles,
	                    CAST(ROUND(igs.avg_total_user_cost, 3) AS DECIMAL(8,3)) AS cost,
	                    igs.avg_user_impact AS impact,
	                    igs.user_seeks + igs.user_scans + igs.system_seeks + igs.system_scans AS need,
	                    COALESCE(igs.last_user_seek, igs.last_user_scan, igs.last_system_seek, igs.last_system_scan) AS lastneeded,
	                    CAST(ROUND((CAST(igs.user_seeks AS NUMERIC(19,6))+CAST(igs.unique_compiles AS NUMERIC(19,6))) * CAST(igs.avg_total_user_cost AS Numeric(19,6)) * CAST(igs.avg_user_impact/100.0 AS NUMERIC(19,6)), 3) AS DECIMAL(19,3)) AS score
                    FROM sys.objects o WITH (NOLOCK)
                    INNER JOIN (
	                    SELECT
		                    object_id,
		                    SUM(CASE WHEN index_id BETWEEN 0 AND 1 THEN row_count ELSE 0 END) AS [rows],
		                    CONVERT(NUMERIC(19,3), CAST(SUM(in_row_reserved_page_count + lob_reserved_page_count + row_overflow_reserved_page_count) AS NUMERIC(19,3))/CAST(128 AS NUMERIC(19,3))) AS sizemb
	                    FROM sys.dm_db_partition_stats ps WITH (NOLOCK)
	                    WHERE ps.index_id BETWEEN 0 AND 1
	                    GROUP BY [object_id]
                    ) p ON o.object_id=p.object_id
                    INNER JOIN sys.schemas s WITH (NOLOCK) ON o.schema_id=s.schema_id
                    INNER JOIN sys.dm_db_missing_index_details id WITH (NOLOCK) ON (o.object_id = id.object_id)
                    INNER JOIN sys.dm_db_missing_index_groups ig WITH (NOLOCK) ON (id.index_handle = ig.index_handle)
                    INNER JOIN sys.dm_db_missing_index_group_stats igs WITH (NOLOCK) ON (ig.index_group_handle = igs.group_handle)
                    WHERE id.database_id=DB_ID();
            ";

            }
        }

    }


}
