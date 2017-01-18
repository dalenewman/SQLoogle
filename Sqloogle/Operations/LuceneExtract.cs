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
using System.IO;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {
    public class LuceneExtract : AbstractOperation {

        private readonly FSDirectory _indexDirectory;

        public LuceneExtract(string folder)
        {
            UseTransaction = false;
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
