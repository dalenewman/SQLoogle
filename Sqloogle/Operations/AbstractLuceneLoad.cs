using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;


namespace Sqloogle.Operations {

    public abstract class AbstractLuceneLoad : AbstractOperation {

        private readonly LuceneWriter _luceneWriter;
        private readonly Dictionary<string, int> _counters = new Dictionary<string, int>();

        public Dictionary<string, LuceneFieldSettings> Schema { get; set; }

        protected AbstractLuceneLoad(string folder)
        {
            _luceneWriter = new LuceneWriter(folder);

            _counters.Add("None", 0);
            _counters.Add("Create", 0);
            _counters.Add("Update", 0);

            Schema = new Dictionary<string, LuceneFieldSettings>();
        }

        public abstract void PrepareSchema();

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            PrepareSchema();

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
                        _luceneWriter.Add(RowToDoc(row));
                        break;
                    case "Delete":
                        _luceneWriter.Delete(row["id"].ToString());
                        break;
                    case "Update":
                        _luceneWriter.Update(row["id"].ToString(), RowToDoc(row));
                        break;
                }
            }
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

        public sealed override void Dispose() {
            Info("Lucene Create: {0}, Update: {1}, and None: {2}.", _counters["Create"], _counters["Update"], _counters["None"]);

            _luceneWriter.Dispose();
            base.Dispose();
        }
    }
}
