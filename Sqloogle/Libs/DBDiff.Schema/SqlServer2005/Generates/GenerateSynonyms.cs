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
    public class GenerateSynonyms
    {
        private Generate root;

        public GenerateSynonyms(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            return @"
                SELECT 
	                SCHEMA_NAME(s.schema_id) AS [Owner],
	                s.[name],
	                s.object_id,
	                s.base_object_name,
	                o.create_date,
	                o.modify_date
                FROM sys.synonyms s
                INNER JOIN sys.objects o ON (s.object_id = o.object_id)
                ORDER BY s.[Name]
                ";
        }

        public void Fill(Database database, string connectionString)
        {
            if (database.Options.Ignore.FilterSynonyms)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(GetSQL(), conn))
                    {
                        conn.Open();
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Synonym item = new Synonym(database) {
                                    Id = (int) reader["object_id"],
                                    Name = reader["Name"].ToString(),
                                    Owner = reader["Owner"].ToString(),
                                    Value = reader["base_object_name"].ToString(),
                                    CreateDate = (DateTime) reader["create_date"],
                                    ModifyDate = (DateTime) reader["modify_date"]
                                };
                                database.Synonyms.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
