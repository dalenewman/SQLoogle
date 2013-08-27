using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Sqloogle.Libs.Rhino.Etl.Core;

namespace Sqloogle {
    public class SqlConnectionChecker : WithLoggingMixin {

        private readonly List<SqlConnectionStringBuilder> _builders = new List<SqlConnectionStringBuilder>();

        public SqlConnectionChecker(IEnumerable<string> connectionStrings, int timeOut = 7) {
            foreach (var connectionString in connectionStrings) {
                _builders.Add(new SqlConnectionStringBuilder(connectionString) { ConnectTimeout = timeOut });
            }
        }

        public bool AllGood() {
            var results = new List<bool>();

            foreach (var builder in _builders) {
                SqlConnection sqlConnection;
                using (sqlConnection = new SqlConnection(builder.ConnectionString)) {
                    try {
                        sqlConnection.Open();
                        results.Add(sqlConnection.State == ConnectionState.Open);
                    }
                    catch (Exception e) {
                        results.Add(false);
                        Error(e, String.Format("Failed to connect to server: {0}, database: {1}.", builder.DataSource, builder.InitialCatalog));
                    }
                }
            }

            var result = results.All(b => b);
            if(result)
                Debug("All databases are online.  Proceed.");
            return result;
        }

    }
}