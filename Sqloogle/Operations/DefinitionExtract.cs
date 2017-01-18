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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {

    public class DefinitionExtract : AbstractOperation {

        public DefinitionExtract() {
            UseTransaction = false;
        }

        private const string CONNECTION_STRING_KEY = "connectionstring";

        private SqlConnectionStringBuilder _connectionStringBuilder;

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var counter = 0;
            foreach (var row in rows) {
                counter++;
                if (counter == 1) {
                    Guard.Against(!row.Contains(CONNECTION_STRING_KEY), "Row must contain connectionstring key.");
                }

                _connectionStringBuilder = new SqlConnectionStringBuilder(row[CONNECTION_STRING_KEY].ToString());
                var sqlConnectionChecker = new SqlConnectionChecker(new[] { _connectionStringBuilder.ConnectionString });

                if (!sqlConnectionChecker.AllGood())
                    continue;

                var results = new Database();
                var subRows = new List<Row>();
                var generator = new Generate() { ConnectionString = _connectionStringBuilder.ConnectionString };

                Debug("Started generating definitions on {0} for {1}", _connectionStringBuilder.DataSource, _connectionStringBuilder.InitialCatalog);

                try {
                    results = generator.Process();
                } catch (Exception e) {
                    Warn("Trouble processing objects from {0}.{1}.\nError Message: {2}.", _connectionStringBuilder.DataSource, _connectionStringBuilder.InitialCatalog, e.Message);
                }

                Debug("Finished generating defs on {0} for {1}", _connectionStringBuilder.DataSource, _connectionStringBuilder.InitialCatalog);

                subRows.AddRange(ToRows(results.Procedures, "StoredProcedures", "Stored Procedure"));
                subRows.AddRange(ToRows(results.Functions, "Functions", "Function"));
                subRows.AddRange(ToRows(results.Tables, "Tables", "Table"));
                subRows.AddRange(ToRows(results.Views, "Views", "View"));
                subRows.AddRange(ToRows(results.Schemas, "Schemas", "Schema"));
                subRows.AddRange(ToRows(results.Synonyms, "Synonyms", "Synonym"));
                subRows.AddRange(ToRows(results.FullText, "FullTextCatalogs", "Full Text Catalog"));

                foreach (var table in results.Tables) {
                    subRows.AddRange(ToRows(table.Indexes, "Indexes", "Index", true));
                    subRows.AddRange(ToRows(table.Triggers, "Triggers", "Trigger", true));
                    foreach (var constraint in table.Constraints) {
                        switch (constraint.Type) {
                            case Constraint.ConstraintType.Check:
                                subRows.AddRange(ToRows(Enumerable.Repeat(constraint, 1), "CheckConstraints", "Check Constraint", true, true));
                                break;
                            case Constraint.ConstraintType.Default:
                                subRows.AddRange(ToRows(Enumerable.Repeat(constraint, 1), "DefaultConstraints", "Default Constraint", true, true));
                                break;
                            case Constraint.ConstraintType.ForeignKey:
                                subRows.AddRange(ToRows(Enumerable.Repeat(constraint, 1), "ForeignKeys", "Foreign Key", true, true));
                                break;
                            case Constraint.ConstraintType.PrimaryKey:
                                subRows.AddRange(ToRows(Enumerable.Repeat(constraint, 1), "PrimaryKeys", "Primary Key", true, true));
                                break;
                            case Constraint.ConstraintType.Unique:
                                subRows.AddRange(ToRows(Enumerable.Repeat(constraint, 1), "UniqueConstraints", "Unique Constraint", true, true));
                                break;
                        }
                    }

                    foreach (var index in table.FullTextIndex)
                        subRows.AddRange(ToRows(Enumerable.Repeat(index, 1), "FullTextIndexes", "Full Text Index", true));
                }

                Info("Found {0} in {1}.", subRows.Count, _connectionStringBuilder.InitialCatalog);

                foreach (var subRow in subRows) {
                    yield return subRow;
                }

            }
        }

        private IEnumerable<Row> ToRows(IEnumerable<SQLServerSchemaBase> dbObjects, string path, string sqlType, bool useParent = false, bool addVersion = false) {

            var rows = new List<Row>();

            foreach (var dbObject in dbObjects) {
                var row = new Row();
                row["sqlscript"] = addVersion ? dbObject.ToSqlAdd() : dbObject.ToSql();
                row["database"] = _connectionStringBuilder.InitialCatalog;
                row["schema"] = dbObject.Owner;
                row["name"] = dbObject.Name;
                row["objectid"] = useParent ? dbObject.Parent.Id : dbObject.Id;
                row["indexid"] = useParent ? dbObject.Id : 0;
                row["path"] = path;
                row["type"] = sqlType;
                row["created"] = dbObject.CreateDate;
                row["modified"] = dbObject.ModifyDate;
                row["use"] = (long)0;
                row["lastused"] = dbObject.ModifyDate;
                rows.Add(row);
            }

            return rows;

        }

    }
}
