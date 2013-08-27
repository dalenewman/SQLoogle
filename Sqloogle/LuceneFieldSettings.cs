using Lucene.Net.Documents;

namespace Sqloogle {
    public class LuceneFieldSettings {
        public Field.Store Store { get; set; }
        public Field.Index Index { get; set; }
        public LuceneFieldSettings(Field.Store store, Field.Index index) {
            Store = store;
            Index = index;
        }
    }
}
