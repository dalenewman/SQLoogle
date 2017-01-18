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
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Options;

#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GenerateDatabase
    {
        private string connectioString;
        private SqlOption objectFilter;

                /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateDatabase(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        public DatabaseInfo Get(Database database)
        {
            DatabaseInfo item = new DatabaseInfo();
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand(DatabaseSQLCommand.GetVersion(database), conn))
                {
                    conn.Open();

                    item.Server = conn.DataSource;
                    item.Database = conn.Database;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string versionValue = reader["Version"] as string;
                            try
                            {
                                // used to use the decimal as well when Azure was 10.25
                                var version = new Version(versionValue);
                                item.VersionNumber = float.Parse(String.Format("{0}.{1}", version.Major, version.Minor), System.Globalization.CultureInfo.InvariantCulture);

                                int? edition = null;
                                if (reader.FieldCount > 1 && !reader.IsDBNull(1))
                                {
                                    int validEdition;
                                    string editionValue = reader[1].ToString();
                                    if (!String.IsNullOrEmpty(editionValue) && int.TryParse(editionValue, out validEdition))
                                    {
                                        edition = validEdition;
                                    }
                                }

                                item.SetEdition(edition);
                            }
                            catch (Exception notAGoodIdeaToCatchAllErrors)
                            {
                                bool useDefaultVersion = false;
//#if DEBUG
//                                useDefaultVersion = IsKeyPushedDown(System.Windows.Forms.Keys.LShiftKey)
//                                    && IsKeyPushedDown(System.Windows.Forms.Keys.RShiftKey);
//#endif

                                var exception = new DBDiff.Schema.Misc.SchemaException(
                                    String.Format("Error parsing ProductVersion. ({0})", versionValue ?? "[null]")
                                    , notAGoodIdeaToCatchAllErrors);
                                
                                if (!useDefaultVersion)
                                {
                                    throw exception;
                                }
                            }
                        }
                    }
                }

                using (SqlCommand command = new SqlCommand(DatabaseSQLCommand.Get(item.Version, database), conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item.Collation = reader["Collation"].ToString();
                            item.HasFullTextEnabled = ((int)reader["IsFulltextEnabled"]) == 1;
                        }
                    }
                }

            }

            return item;
        }

//#if DEBUG
//        // http://pinvoke.net/default.aspx/user32/GetAsyncKeyState.html
//        [DllImport("user32.dll")]
//        static extern ushort GetAsyncKeyState(int vKey);

//        public static bool IsKeyPushedDown(System.Windows.Forms.Keys vKey)
//        {
//            return 0 != (GetAsyncKeyState((int)vKey) & 0x8000);
//        }
//#endif    
    }
}
