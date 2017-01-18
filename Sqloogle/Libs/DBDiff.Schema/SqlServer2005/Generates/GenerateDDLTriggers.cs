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
    public class GenerateDDLTriggers
    {
        private Generate root;

        public GenerateDDLTriggers(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            string sql = "";
            sql += "SELECT OBJECT_DEFINITION(T.object_id) AS Text,T.name,is_disabled,is_not_for_replication,is_instead_of_trigger ";
            sql += "FROM sys.triggers T ";
            sql += "WHERE T.parent_id = 0 AND T.parent_class = 0";
            return sql;
        }

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterDDLTriggers)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Trigger trigger = new Trigger(database);
                                trigger.Text = reader["Text"].ToString();
                                trigger.Name = reader["Name"].ToString();
                                trigger.InsteadOf = (bool)reader["is_instead_of_trigger"];
                                trigger.IsDisabled = (bool)reader["is_disabled"];
                                trigger.IsDDLTrigger = true;
                                trigger.NotForReplication = (bool)reader["is_not_for_replication"];
                                trigger.Owner = "";
                                database.DDLTriggers.Add(trigger);
                            }
                        }
                    }
                }
            }
        }
    }
}
