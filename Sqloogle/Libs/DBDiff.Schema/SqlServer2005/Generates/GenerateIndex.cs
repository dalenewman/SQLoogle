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
using Sqloogle.Libs.DBDiff.Schema.Events;
using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates.SQLCommands;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates.Util;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates
{
    public class GenerateIndex
    {
        private Generate root;

        public GenerateIndex(Generate root)
        {
            this.root = root;
        }

        public void Fill(Database database, string connectionString)
        {
            int indexid = 0;
            int parentId = 0;
            bool change = false;
            string type;
            ISchemaBase parent = null;
            root.RaiseOnReading(new ProgressEventArgs("Reading Index...", Constants.READING_INDEXES));                      
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(IndexSQLCommand.Get(database.Info.Version), conn))
                {
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Index item = null;
                        while (reader.Read())
                        {
                            root.RaiseOnReadingOne(reader["Name"]);
                            type = reader["ObjectType"].ToString().Trim();
                            if (parentId != (int)reader["object_id"])
                            {
                                parentId = (int)reader["object_id"];
                                if (type.Equals("V"))
                                    parent = database.Views.Find(parentId);
                                else
                                    parent = database.Tables.Find(parentId);
                                change = true;
                            }
                            else
                                change = false;
                            if ((indexid != (int)reader["Index_id"]) || (change))
                            {
                                item = new Index(parent);
                                item.Name = reader["Name"].ToString();
                                item.Owner = parent.Owner;
                                item.Type = (Index.IndexTypeEnum)(byte)reader["type"];
                                item.Id = (int)reader["Index_id"];
                                item.IgnoreDupKey = (bool)reader["ignore_dup_key"];
                                item.IsAutoStatistics = (bool)reader["NoAutomaticRecomputation"];
                                item.IsDisabled = (bool)reader["is_disabled"];
                                item.IsPrimaryKey = (bool)reader["is_primary_key"];
                                item.IsUniqueKey = (bool)reader["is_unique"];
                                if (database.Options.Ignore.FilterIndexRowLock)
                                {
                                    item.AllowPageLocks = (bool)reader["allow_page_locks"];
                                    item.AllowRowLocks = (bool)reader["allow_row_locks"];
                                }
                                if (database.Options.Ignore.FilterIndexFillFactor)
                                {
                                    item.FillFactor = (byte)reader["fill_factor"];
                                    item.IsPadded = (bool)reader["is_padded"];
                                }
                                if ((database.Options.Ignore.FilterTableFileGroup) && (item.Type != Index.IndexTypeEnum.XML))
                                    item.FileGroup = reader["FileGroup"].ToString();

                                if ((database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008) && (database.Options.Ignore.FilterIndexFilter))
                                {
                                    item.FilterDefintion = reader["FilterDefinition"].ToString();
                                }
                                indexid = (int)reader["Index_id"];
                                if (type.Equals("V"))
                                    ((View)parent).Indexes.Add(item);
                                else
                                    ((Table)parent).Indexes.Add(item);
                            }
                            IndexColumn ccon = new IndexColumn(item.Parent);
                            ccon.Name = reader["ColumnName"].ToString();
                            ccon.IsIncluded = (bool)reader["is_included_column"];
                            ccon.Order = (bool)reader["is_descending_key"];
                            ccon.Id = (int)reader["column_id"];
                            ccon.KeyOrder = (byte)reader["key_ordinal"];
                            ccon.DataTypeId = (int)reader["user_type_id"];
                            if ((!ccon.IsIncluded) || (ccon.IsIncluded && database.Options.Ignore.FilterIndexIncludeColumns))
                                item.Columns.Add(ccon);
                        }
                    }
                }
            }
        }
    }
}
