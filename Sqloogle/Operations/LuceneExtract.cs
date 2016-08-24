﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {
    public class LuceneExtract : AbstractOperation {

        private readonly FSDirectory _indexDirectory;

        public LuceneExtract(string folder) {
            _indexDirectory = FSDirectory.Open(new DirectoryInfo(folder));
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            if (_indexDirectory == null)
                yield break;

            IndexReader reader;

            try {
                reader = IndexReader.Open(_indexDirectory, true);
            } catch (Exception) {
                Warn("Failed to open lucene index in {0}.", _indexDirectory.Directory.FullName);
                yield break;
            }

            using (reader) {
                var docCount = reader.NumDocs();
                Info("Found {0} documents in lucene index.", docCount);

                for (var i = 0; i < docCount; i++) {

                    if (reader.IsDeleted(i))
                        continue;

                    var doc = reader.Document(i);
                    var row = new Row();
                    foreach (var field in doc.GetFields().Where(field => field.IsStored)) {
                        switch (field.Name) {
                            case "dropped":
                                row[field.Name] = Convert.ToBoolean(field.StringValue);
                                break;
                            default:
                                row[field.Name] = field.StringValue;
                                break;
                        }

                    }
                    yield return row;

                }
            }




        }

        public override sealed void Dispose() {

            Debug("Lucene Closing directory.");
            _indexDirectory.Dispose();

            base.Dispose();
        }

    }
}
