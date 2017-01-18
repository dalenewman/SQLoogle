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
using System.Collections.Generic;
using System.Data;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {

    public class ReportingServicesExtract : AbstractOperation {

        public ReportingServicesExtract() {
            UseTransaction = false;
        }

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
