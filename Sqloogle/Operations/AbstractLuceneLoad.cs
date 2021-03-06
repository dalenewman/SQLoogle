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
using Lucene.Net.Documents;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;


namespace Sqloogle.Operations {

    public abstract class AbstractLuceneLoad : AbstractOperation {
        private readonly string _folder;

        private readonly Dictionary<string, int> _counters = new Dictionary<string, int>();

        public Dictionary<string, LuceneFieldSettings> Schema { get; set; }

        protected AbstractLuceneLoad(string folder)
        {
            UseTransaction = false;
            _folder = folder;
            _counters.Add("None", 0);
            _counters.Add("Create", 0);
            _counters.Add("Update", 0);

            Schema = new Dictionary<string, LuceneFieldSettings>();
        }

        public abstract void PrepareSchema();

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            PrepareSchema();

            using (var writer = new LuceneWriter(_folder)) {
                foreach (var row in rows) {

                    if (row["action"] == null) {
                        throw new InvalidOperationException("There is no action column.  A valid action is None, Create, or Update!");
                    }

                    var action = row["action"].ToString();
                    row.Remove("action");

                    _counters[action] += 1;

                    switch (action) {
                        case "None":
                            break;
                        case "Create":
                            writer.Add(RowToDoc(row));
                            break;
                        case "Delete":
                            writer.Delete(row["id"].ToString());
                            break;
                        case "Update":
                            writer.Update(row["id"].ToString(), RowToDoc(row));
                            break;
                    }
                }

                writer.Commit();
                writer.Optimize();
            }

            Info("Lucene Create: {0}, Update: {1}, and None: {2}.", _counters["Create"], _counters["Update"], _counters["None"]);
            yield break;
        }

        private Document RowToDoc(Row row) {
            var doc = new Document();
            foreach (var column in row.Columns) {
                if (Schema.ContainsKey(column)) {
                    doc.Add(new Field(column.ToLower(), row[column].ToString(), Schema[column].Store, Schema[column].Index));
                } else {
                    doc.Add(new Field(column.ToLower(), row[column].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                }
            }
            return doc;
        }
    }
}
