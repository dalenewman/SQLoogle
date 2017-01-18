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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Rhino.Etl.Core;

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