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
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates.SQLCommands;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GenerateUsers
    {
        private Generate root;

        public GenerateUsers(Generate root)
        {
            this.root = root;
        }

        public void Fill(Database database, string connectioString)
        {
            string type;
            if ((database.Options.Ignore.FilterUsers) || (database.Options.Ignore.FilterRoles))
            {
                using (SqlConnection conn = new SqlConnection(connectioString))
                {
                    using (SqlCommand command = new SqlCommand(UserSQLCommand.Get(database.Info.Version), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                type = reader["type"].ToString();
                                if (database.Options.Ignore.FilterUsers && (type.Equals("S") || type.Equals("U")))
                                {
                                    User item = new User(database);
                                    item.Id = (int)reader["principal_id"];
                                    item.Name = reader["name"].ToString();
                                    item.Login = reader["Login"].ToString();
                                    item.Owner = reader["default_schema_name"].ToString();
                                    database.Users.Add(item);
                                }
                                if (database.Options.Ignore.FilterRoles && (type.Equals("A") || type.Equals("R")))
                                {
                                    Role item = new Role(database);
                                    item.Id = (int)reader["principal_id"];
                                    item.Name = reader["name"].ToString();
                                    item.Owner = reader["default_schema_name"].ToString();
                                    item.Password = "";
                                    item.IsSystem = (Boolean)reader["is_fixed_role"];
                                    if (type.Equals("A"))
                                        item.Type = Role.RoleTypeEnum.ApplicationRole;
                                    else
                                        item.Type = Role.RoleTypeEnum.DatabaseRole;
                                    database.Roles.Add(item);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
