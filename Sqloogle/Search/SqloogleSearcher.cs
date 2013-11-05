using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Sqloogle.Libs.NLog;
using Sqloogle.Utilities;

namespace Sqloogle.Search
{
    public class SqloogleSearcher : IScriptSearcher
    {
        private readonly StandardAnalyzer _analyzer;
        private readonly Lucene.Net.Store.Directory _directory;
        private readonly Logger _logger = LogManager.GetLogger("Script Searcher");
        private readonly MultiFieldQueryParser _parser;
        private readonly int _resultsLimit;
        private readonly string _indexPath;
        private readonly IndexSearcher _searcher;

        public SqloogleSearcher(string indexPath, int resultsLimit = 50)
        {
            _resultsLimit = resultsLimit;
            _indexPath = indexPath;

            var fields = new[] {
                "server",
                "database",
                "schema",
                "name",
                "type",
                "sql",
                "dropped",
                "use",
                "lastused"
            };

            var boosts = new Dictionary<string, float> {
                { "server", .5F },
                { "database", .4F },
                { "schema", .3F },
                { "name", .2F },
                { "type", .1F },
                { "sql", .0F },
                { "dropped", .0F },
                { "use", .0F },
                { "lastused", .0F}
            };

            _directory = FSDirectory.Open(new DirectoryInfo(_indexPath));
            _searcher = new IndexSearcher(_directory, true);
            _analyzer = new StandardAnalyzer(Version.LUCENE_30);
            _parser = new MultiFieldQueryParser(Version.LUCENE_30, fields, _analyzer, boosts) {
                DefaultOperator = QueryParser.AND_OPERATOR
            };

            _logger.Trace("Searcher is ready.");
        }

        #region IScriptSearcher Members

        public IEnumerable<IDictionary<string, string>> Search(string q)
        {
            q = q.ToLower();

            // by default, results will not include dropped objects, but you can add dropped:? to over-ride this
            if (!q.Contains("dropped:"))
                q = string.Format("({0}) -dropped:true", q);

            var query = _parser.Parse(q);

            var topFieldCollector = TopFieldCollector.Create(
                sort: Sort.RELEVANCE,
                numHits: _resultsLimit,
                fillFields: false,
                trackDocScores: true,
                trackMaxScore: false,
                docsScoredInOrder: false
            );

            _searcher.Search(query, topFieldCollector);

            var topDocs = topFieldCollector.TopDocs();

            if (topDocs == null)
                return Enumerable.Repeat(new Dictionary<string, string>(), 0);

            var results = topDocs.ScoreDocs.Select(hit => Docs.DocToDict(_searcher.Doc(hit.Doc), hit.Score)).ToArray();
            _logger.Debug("Search of '{0}' return {1} results.", q, results.Count());
            return results;
        }

        public void Close()
        {
            _analyzer.Close();
            _analyzer.Dispose();
            _searcher.Dispose();
            _directory.Dispose();

            _logger.Info("Searcher is closed.");
        }

        #endregion

        public IDictionary<string,string> Find(string id)
        {
            var keywordAnalyzer = new KeywordAnalyzer();
            var parser = new QueryParser(Version.LUCENE_30, "id", keywordAnalyzer);
            var query = parser.Parse(id);
            var scoreDocs = _searcher.Search(query, 1).ScoreDocs;

            if (scoreDocs != null && scoreDocs.Length > 0)
                return Docs.DocToDict(_searcher.Doc(scoreDocs[0].Doc), 1f);

            return null;
        }
    }
}