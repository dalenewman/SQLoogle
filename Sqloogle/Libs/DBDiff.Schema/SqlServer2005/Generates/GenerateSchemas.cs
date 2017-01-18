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
using System.Data.SqlClient;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GenerateSchemas
    {
        private Generate root;

        public GenerateSchemas(Generate root)
        {
            this.root = root;
        }
        
        private static string GetSQL()
        {
            string sql;
            sql = "SELECT S1.name,S1.schema_id, S2.name AS Owner, S2.create_date, S2.modify_date FROM sys.schemas S1 ";
            sql += "INNER JOIN sys.database_principals S2 ON S2.principal_id = S1.principal_id ";
            return sql;
        }

        public void Fill(Database database, string connectioString)
        {
            if (database.Options.Ignore.FilterSchema)
            {
                using (SqlConnection conn = new SqlConnection(connectioString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Model.Schema item = new Model.Schema(database);
                                item.Id = (int)reader["schema_id"];
                                item.Name = reader["name"].ToString();
                                item.Owner = reader["owner"].ToString();
                                item.CreateDate = (DateTime) reader["create_date"];
                                item.ModifyDate = (DateTime) reader["modify_date"];
                                database.Schemas.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
