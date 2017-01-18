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
using System.Text;
using System.Data.SqlClient;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GeneratePartitionFunctions
    {
        private Generate root;

        public GeneratePartitionFunctions(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL()
        {
            string sql;
            sql = "select PRV.value, T.name AS TypeName, PP.max_length, PP.precision, PP.scale, PF.Name, PF.function_id, fanout, boundary_value_on_right AS IsRight ";
            sql += "from sys.partition_functions PF ";
            sql += "INNER JOIN sys.partition_parameters PP ON PP.function_id = PF.function_id ";
            sql += "INNER JOIN sys.types T ON T.system_type_id = PP.system_type_id ";
            sql += "INNER JOIN sys.partition_range_values PRV ON PRV.parameter_id = PP.parameter_id and PP.function_id = PRV.function_id ";
            sql += "ORDER BY PP.function_id, PRV.parameter_id, boundary_id";
            return sql;
        }
        
        private static string ToHex(byte[] stream)
        {
            StringBuilder sHex = new StringBuilder(2 * stream.Length);
            for (int i = 0; i < stream.Length; i++)
                sHex.AppendFormat("{0:X2} ", stream[i]);
            return "0x" + sHex.ToString().Replace(" ", String.Empty);
        }

        public void Fill(Database database, string connectioString)
        {
            int lastObjectId = 0;
            PartitionFunction item = null;
            if (database.Options.Ignore.FilterPartitionFunction)
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
                                if (lastObjectId != (int)reader["function_id"])
                                {
                                    lastObjectId = (int)reader["function_id"];
                                    item = new PartitionFunction(database);
                                    item.Id = (int)reader["function_id"];
                                    item.Name = reader["name"].ToString();
                                    item.IsBoundaryRight = (bool)reader["IsRight"];
                                    item.Precision = (byte)reader["precision"];
                                    item.Scale = (byte)reader["scale"];
                                    item.Size = (short)reader["max_length"];
                                    item.Type = reader["TypeName"].ToString();
                                    database.PartitionFunctions.Add(item);
                                }
                                if (item.Type.Equals("binary") || item.Type.Equals("varbinary"))
                                    item.Values.Add(ToHex((byte[])reader["value"]));
                                else
                                    item.Values.Add(reader["value"].ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}
