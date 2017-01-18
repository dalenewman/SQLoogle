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
using System.Data.SqlClient;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GenerateRules
    {
        private Generate root;

        public GenerateRules(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            string sql = "select obj.object_id, Name, SCHEMA_NAME(obj.schema_id) AS Owner, ISNULL(smobj.definition, ssmobj.definition) AS [Definition] from sys.objects obj  ";
            sql += "LEFT OUTER JOIN sys.sql_modules AS smobj ON smobj.object_id = obj.object_id ";
            sql += "LEFT OUTER JOIN sys.system_sql_modules AS ssmobj ON ssmobj.object_id = obj.object_id ";
            sql += "where obj.type='R'";
            return sql;
        }

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterRules)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Rule item = new Rule(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = reader["Name"].ToString();
                                item.Owner = reader["Owner"].ToString();
                                item.Text = reader["Definition"].ToString();
                                database.Rules.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
