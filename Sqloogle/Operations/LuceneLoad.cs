using Lucene.Net.Documents;

namespace Sqloogle.Operations {
    public class LuceneLoad : AbstractLuceneLoad {

        public LuceneLoad(string folder) : base(folder) {}

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