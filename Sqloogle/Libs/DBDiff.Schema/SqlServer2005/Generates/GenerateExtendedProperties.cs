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
using System.Data.SqlClient;
using Sqloogle.Libs.DBDiff.Schema.Errors;
using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Interfaces;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GenerateExtendedProperties
    {
        private Generate root;

        public GenerateExtendedProperties(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            string sql = "SELECT DISTINCT T2.Name as ParentName, S1.name AS OwnerType, T.name AS TypeName, O.type, A.name AS AssemblyName, EP.*,S.name as Owner, O.name AS ObjectName, I1.name AS IndexName FROM sys.extended_properties EP ";
            sql += "LEFT JOIN sys.objects O ON O.object_id = EP.major_id ";
            sql += "LEFT JOIN sys.schemas S ON S.schema_id = O.schema_id ";
            sql += "LEFT JOIN sys.assemblies A ON A.assembly_id = EP.major_id ";
            sql += "LEFT JOIN sys.types T ON T.user_type_id = EP.major_id ";
            sql += "LEFT JOIN sys.schemas S1 ON S1.schema_id = T.schema_id ";
            sql += "LEFT JOIN sys.indexes I1 ON I1.index_id = EP.minor_id AND I1.object_id = O.object_ID AND class = 7 ";
            sql += "LEFT JOIN sys.tables T2 ON T2.object_id = O.parent_object_id AND class = 1";
            sql += "ORDER BY major_id";
            return sql;
        }

        private static string GetTypeDescription(string type)
        {
            if (type.Equals("PC")) return "PROCEDURE";
            if (type.Equals("P")) return "PROCEDURE";
            if (type.Equals("V")) return "VIEW";
            if (type.Equals("U")) return "TABLE";
            if (type.Equals("TR")) return "TRIGGER";
            if (type.Equals("TA")) return "TRIGGER";
            if (type.Equals("FS")) return "FUNCTION";
            if (type.Equals("FN")) return "FUNCTION";
            if (type.Equals("IF")) return "FUNCTION";
            if (type.Equals("TF")) return "FUNCTION";
            return "";
        }

        public void Fill(Database database, string connectionString, List<MessageLog> messages)
        {
            ISQLServerSchemaBase parent;
            try
            {
                if (database.Options.Ignore.FilterExtendedPropertys)
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
                                    ExtendedProperty item = new ExtendedProperty(null);
                                    if (((byte)reader["Class"]) == 5)
                                    {
                                        item.Level0type = "ASSEMBLY";
                                        item.Level0name = reader["AssemblyName"].ToString();
                                    }
                                    if (((byte)reader["Class"]) == 1)
                                    {
                                        string ObjectType = GetTypeDescription(reader["type"].ToString().Trim());
                                        item.Level0type = "SCHEMA";
                                        item.Level0name = reader["Owner"].ToString();
                                        if (!ObjectType.Equals("TRIGGER"))
                                        {
                                            item.Level1name = reader["ObjectName"].ToString();
                                            item.Level1type = ObjectType;
                                        }
                                        else
                                        {
                                            item.Level1type = "TABLE";
                                            item.Level1name = reader["ParentName"].ToString();
                                            item.Level2name = reader["ObjectName"].ToString();
                                            item.Level2type = ObjectType;
                                        }
                                    }
                                    if (((byte)reader["Class"]) == 6)
                                    {
                                        item.Level0type = "SCHEMA";
                                        item.Level0name = reader["OwnerType"].ToString();
                                        item.Level1name = reader["TypeName"].ToString();
                                        item.Level1type = "TYPE";
                                    }
                                    if (((byte)reader["Class"]) == 7)
                                    {
                                        item.Level0type = "SCHEMA";
                                        item.Level0name = reader["Owner"].ToString();
                                        item.Level1type = "TABLE";
                                        item.Level1name = reader["ObjectName"].ToString();
                                        item.Level2type = reader["class_desc"].ToString();
                                        item.Level2name = reader["IndexName"].ToString();                                        
                                    } 
                                    item.Value = reader["Value"].ToString();
                                    item.Name = reader["Name"].ToString();
                                    parent = ((ISQLServerSchemaBase)database.Find(item.FullName));
                                    if (parent != null)
                                    {
                                        item.Parent = (ISchemaBase)parent;
                                        parent.ExtendedProperties.Add(item);
                                    }
                                    else
                                        messages.Add(new MessageLog(item.FullName + " not found in extended properties.", "", MessageLog.LogType.Error));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                messages.Add(new MessageLog(ex.Message,ex.StackTrace, MessageLog.LogType.Error));
            }
        }
    }
}
