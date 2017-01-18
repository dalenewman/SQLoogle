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
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations.Support {

    public class SqlSubOperation : AbstractOperation {

        private const string COLUMN_WITH_CONNECTION_STRING = "connectionstring";
        private readonly string _columnWithSql;

        public SqlSubOperation(string columnWithSql)
        {
            UseTransaction = false;
            _columnWithSql = columnWithSql;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                foreach (var subRow in GetSubOperation(row).Execute(null)) {
                    foreach (var column in row.Columns)
                        if (column != _columnWithSql)
                            subRow.Add(column, row[column]);

                    yield return subRow;
                }
            }

        }

        protected IOperation GetSubOperation(Row row)
        {
            Guard.Against(!row.Contains(COLUMN_WITH_CONNECTION_STRING), string.Format("Rows must contain \"{0}\" key.", COLUMN_WITH_CONNECTION_STRING));
            Guard.Against(!row.Contains(_columnWithSql), string.Format("Rows must contain \"{0}\" key.", _columnWithSql));

            var connectionString = row[COLUMN_WITH_CONNECTION_STRING].ToString();
            var sql = row[_columnWithSql].ToString();
            return new SqlOperation(connectionString, sql);            
        }
    }
}
