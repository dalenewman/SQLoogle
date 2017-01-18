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
    public class GenerateFullText
    {
        private Generate root;

        public GenerateFullText(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            string sql = "SELECT S.Name as Owner,F.name AS FileGroupName, fulltext_catalog_id, FC.Name, path, FC.is_default, is_accent_sensitivity_on ";
            sql += "FROM sys.fulltext_catalogs FC ";
            sql += "LEFT JOIN sys.filegroups F ON F.data_space_id = FC.data_space_id ";
            sql += "INNER JOIN sys.schemas S ON S.schema_id = FC.principal_id";
            return sql;
        }

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterFullText)
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
                                FullText item = new FullText(database);
                                item.Id = (int)reader["fulltext_catalog_id"];
                                item.Name = reader["Name"].ToString();
                                item.Owner = reader["Owner"].ToString();
                                item.IsAccentSensity = (bool)reader["is_accent_sensitivity_on"];
                                item.IsDefault = (bool)reader["is_default"];
                                if (!reader.IsDBNull(reader.GetOrdinal("path")))
                                    item.Path = reader["path"].ToString().Substring(0, reader["path"].ToString().Length - item.Name.Length);
                                if (!reader.IsDBNull(reader.GetOrdinal("FileGroupName")))
                                    item.FileGroupName = reader["FileGroupName"].ToString();
                                database.FullText.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
