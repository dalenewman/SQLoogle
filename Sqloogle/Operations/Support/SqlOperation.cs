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
using Rhino.Etl.Core;

namespace Sqloogle.Operations.Support
{

    /// <summary>
    /// Generic input command operation that takes the
    /// connection string and sql you want to run.  Use
    /// this when your program is creating the connection
    /// string and sql (you don't know it up front).
    /// </summary>
    public class SqlOperation : InputOperation
    {
        private readonly string _sql;

        public SqlOperation(string connectionString, string sql) : base(connectionString)
        {
            UseTransaction = false;
            _sql = sql;
        }

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandText = _sql;
            cmd.CommandTimeout = 0;
        }
    }
}