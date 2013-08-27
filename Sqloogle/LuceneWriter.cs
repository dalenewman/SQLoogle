using System;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Sqloogle.Libs.Rhino.Etl.Core;
using Version = Lucene.Net.Util.Version;

namespace Sqloogle
{
    public class LuceneWriter : WithLoggingMixin, IIndexWriter, IDisposable {
        private readonly string _folder;
        private readonly FSDirectory _indexDirectory;
        private readonly StandardAnalyzer _standardAnalyzer;
        private readonly IndexWriter _indexWriter;

        public LuceneWriter(string folder) {
            _folder = folder;
            _indexDirectory = FSDirectory.Open(new DirectoryInfo(folder));
            _standardAnalyzer = new StandardAnalyzer(Version.LUCENE_30);
            _indexWriter = new IndexWriter(_indexDirectory, _standardAnalyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        public void Clean() {
            Info("Cleaning Lucene index at {0}", _folder);
            _indexWriter.DeleteAll();
        }

        public void Add(object doc) {
            _indexWriter.AddDocument((Document) doc);
        }

        public void Delete(string id) {
            _indexWriter.DeleteDocuments(new Term("id",id));
        }

        public void Update(string id, object doc)
        {
            _indexWriter.UpdateDocument(new Term("id", id), (Document)doc);
        }

        public void Dispose() {
            Info("Lucene Committing.");
            _indexWriter.Commit();

            Info("Lucene Optimizing.");
            _indexWriter.Optimize();

            _indexWriter.Dispose();
            _indexDirectory.Dispose();
            _standardAnalyzer.Close();
            _standardAnalyzer.Dispose();
        }
    }
}