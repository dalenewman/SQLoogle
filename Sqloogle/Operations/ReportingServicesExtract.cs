using System.Collections.Generic;
using System.Data;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {

    public class ReportingServicesExtract : AbstractOperation {

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                var connectionString = row["connectionstring"].ToString();
                var sqlConnectionChecker = new SqlConnectionChecker(new[] { connectionString });

                if (!sqlConnectionChecker.AllGood())
                    continue;

                foreach (var subRow in new InternalOperation(connectionString).Execute(null)) {
                    foreach (var column in row.Columns)
                        subRow.Add(column, row[column]);
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

                DECLARE @Columns AS INT;

                SELECT @Columns = COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS WITH (NOLOCK)
                WHERE TABLE_SCHEMA = 'dbo'
                AND TABLE_NAME = 'Catalog'
                AND COLUMN_NAME IN ('Name','CreationDate','ModifiedDate','Path','Content','Type');

                IF @Columns = 6
	                SELECT
		                [name],
		                CreationDate AS created,
		                ModifiedDate AS modified,
		                [path],
		                CAST(CAST([Content] AS VARBINARY(MAX)) AS XML) AS rdl
	                FROM [Catalog] WITH (NOLOCK)
	                WHERE [Content] IS NOT NULL
	                AND [Type] = 2;
                ELSE
	                SELECT
		                CAST(NULL AS NVARCHAR(425)) AS [name],
		                CAST(NULL AS DATETIME) AS created,
		                CAST(NULL AS DATETIME) AS modified,
		                CAST(NULL AS NVARCHAR(425)) AS [path],
		                CAST(NULL AS XML) AS rdl
	                WHERE 1=2;
            ";

            }
        }

    }


}
