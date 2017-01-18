﻿#region license
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
using Sqloogle.Libs.DBDiff.Schema.Events;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates.Util;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Interfaces;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Options;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GenerateTextObjects
    {
        private Generate root;

        public GenerateTextObjects(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL(SqlOption options)
        {
            string filter = "";
            string sql = "";
            sql += "SELECT O.name, O.type, M.object_id, OBJECT_DEFINITION(M.object_id) AS Text FROM sys.sql_modules M ";
            sql += "INNER JOIN sys.objects O ON O.object_id = M.object_id ";
            sql += "WHERE ";
            if (options.Ignore.FilterStoreProcedure)
                filter += "O.type = 'P' OR ";
            if (options.Ignore.FilterView)
                filter += "O.type = 'V' OR ";
            if (options.Ignore.FilterTrigger)
                filter += "O.type = 'TR' OR ";
            if (options.Ignore.FilterFunction)
                filter += "O.type IN ('IF','FN','TF') OR ";
            filter = filter.Substring(0, filter.Length - 4);
            return sql + filter;
        }

        public void Fill(Database database, string connectionString)
        {
            ICode code = null;
            try
            {
                if ((database.Options.Ignore.FilterStoreProcedure) || (database.Options.Ignore.FilterView) || (database.Options.Ignore.FilterFunction) || (database.Options.Ignore.FilterTrigger))
                {
                    root.RaiseOnReading(new ProgressEventArgs("Reading Text Objects...", Constants.READING_TEXTOBJECTS));
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(GetSQL(database.Options), conn))
                        {
                            conn.Open();
                            command.CommandTimeout = 0;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    code = null;
                                    root.RaiseOnReadingOne(reader["name"]);
                                    string type = reader["Type"].ToString().Trim();
                                    string name = reader["name"].ToString();
                                    string definition = reader["Text"].ToString();
                                    int id = (int)reader["object_id"];
                                    if (type.Equals("V"))
                                        code = (ICode)database.Views.Find(id);

                                    if (type.Equals("TR"))
                                        code = (ICode)database.Find(id);

                                    if (type.Equals("P"))
                                        ((ICode)database.Procedures.Find(id)).Text = GetObjectDefinition(type, name, definition);

                                    if (type.Equals("IF") || type.Equals("FN") || type.Equals("TF"))
                                        code = (ICode)database.Functions.Find(id);

                                    if (code != null)
                                        code.Text = reader["Text"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetObjectDefinition(string type, string name, string definition)
        {
            string rv = definition;

            string sqlDelimiters = @"(\r|\n|\s)*?";
            System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline;
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"CREATE" + sqlDelimiters + @"PROC(EDURE)?" + sqlDelimiters + @"(\w+\.|\[\w+\]\.)?\[?(?<spname>\w+)\]?" + sqlDelimiters, options);
            switch (type)
            {
                case "P":
                    System.Text.RegularExpressions.Match match = re.Match(definition);
                    if (match != null && match.Success)
                    {
                        // Try to replace the name saved in the definition when the object was created by the one used for the object in sys.object
                        string oldName = match.Groups["spname"].Value;
                        //if (String.IsNullOrEmpty(oldName)) System.Diagnostics.Debugger.Break();
                        if (String.Compare(oldName, name) != 0)
                        {
                            rv = rv.Replace(oldName, name);
                        }
                    }
                    break;
                default:
                    //TODO : Add the logic used for other objects than procedures
                    break;
            }

            return rv;
        }
    }
}
