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
using System.Configuration;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations.Support {

    /// <summary>
    /// Generic input command operation that takes the
    /// actual connection string itself, rather than a
    /// connection string from the configuration, or a
    /// connection string settings object.
    /// </summary>
    public abstract class InputOperation : InputCommandOperation     {

        private const string PROVIDER = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private static string _connectionString;

        protected InputOperation(string connectionString) : base(GetConnectionStringSettings(connectionString))
        {
            UseTransaction = false;
        }

        private static ConnectionStringSettings GetConnectionStringSettings(string connectionString)
        {
            _connectionString = connectionString;
            return new ConnectionStringSettings {
                ConnectionString = _connectionString,
                ProviderName = PROVIDER,
            };
        }

    }
}
