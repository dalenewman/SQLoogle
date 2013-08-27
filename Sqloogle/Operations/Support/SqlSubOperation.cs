using System.Collections.Generic;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations.Support {

    public class SqlSubOperation : AbstractOperation {

        private const string COLUMN_WITH_CONNECTION_STRING = "connectionstring";
        private readonly string _columnWithSql;

        public SqlSubOperation(string columnWithSql) {
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
