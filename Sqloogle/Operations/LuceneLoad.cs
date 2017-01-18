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
using Lucene.Net.Documents;

namespace Sqloogle.Operations {
    public class LuceneLoad : AbstractLuceneLoad {

        public LuceneLoad(string folder) : base(folder)
        {
            UseTransaction = false;
        }

        public override void PrepareSchema() {
            Schema["id"] = new LuceneFieldSettings(Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
            Schema["use"] = new LuceneFieldSettings(Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
            Schema["count"] = new LuceneFieldSettings(Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
            Schema["dropped"] = new LuceneFieldSettings(Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
            Schema["score"] = new LuceneFieldSettings(Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
            Schema["created"] = new LuceneFieldSettings(Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
            Schema["modified"] = new LuceneFieldSettings(Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
            Schema["lastused"] = new LuceneFieldSettings(Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
            Schema["lastneeded"] = new LuceneFieldSettings(Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
            Schema["sql"] = new LuceneFieldSettings(Field.Store.NO, Field.Index.ANALYZED);
            Schema["sqlscript"] = new LuceneFieldSettings(Field.Store.YES, Field.Index.NO);
        }
    }
}