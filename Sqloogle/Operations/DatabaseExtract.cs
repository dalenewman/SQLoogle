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
using System.Data.SqlClient;
using Rhino.Etl.Core;
using Sqloogle.Operations.Support;

namespace Sqloogle.Operations {
    /// <summary>
    /// This is SQLoogle's (SQL Server) Database Extract operation. 
    /// It implements the PrepareCommand and CreateRowFromReader methods.  
    /// It creates a database specific connection string for each database.
    /// </summary>
    public class DatabaseExtract : InputOperation {
        private readonly string _connectionString;

        public DatabaseExtract(string connectionString) : base(connectionString) {
            UseTransaction = false;
            _connectionString = connectionString;
        }

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
