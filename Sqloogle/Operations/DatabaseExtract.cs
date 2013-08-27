using System.Data;
using System.Data.SqlClient;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {
    /// <summary>
    /// This is SQLoogle's (SQL Server) Database Extract operation. 
    /// It implements the PrepareCommand and CreateRowFromReader methods.  
    /// It creates a database specific connection string for each database.
    /// </summary>
    public class DatabaseExtract : InputOperation {
        private readonly string _connectionString;

        public DatabaseExtract(string connectionString) : base(connectionString) { _connectionString = connectionString; }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = @"/* SQLoogle */
                USE master;

                SELECT
                    database_id AS databaseid
                    ,[Name] AS [database]
                    ,Compatibility_Level AS compatibilitylevel
                FROM sys.databases WITH (NOLOCK)
                WHERE [state] = 0
                AND [user_access] = 0
                AND [is_in_standby] = 0
                AND compatibility_level >= 80
                ORDER BY [name] ASC;
            ";
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = Row.FromReader(reader);
            row["connectionstring"] = GetDatabaseSpecificConnectionString(row);
            return row;
        }

        protected string GetDatabaseSpecificConnectionString(Row row) {
            var builder = new SqlConnectionStringBuilder(_connectionString) {
                InitialCatalog = row["database"].ToString()
            };
            return builder.ConnectionString;
        }

    }
}
