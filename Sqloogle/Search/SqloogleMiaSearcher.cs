using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Sqloogle.Libs.NLog;
using Sqloogle.Utilities;
using Version = Lucene.Net.Util.Version;

namespace Sqloogle.Search
{
    public class SqloogleMiaSearcher : IScriptSearcher
    {
        private readonly StandardAnalyzer _analyzer;
        private readonly Lucene.Net.Store.Directory _directory;
        private readonly Logger _logger = LogManager.GetLogger("Missing Index Searcher");
        private readonly QueryParser _parser;
        private readonly IndexSearcher _searcher;
        private readonly int _resultsLimit;

        public SqloogleMiaSearcher(string indexPath, int resultsLimit = 50)
        {
            _resultsLimit = resultsLimit;
            var fields = new[] {
                "server",
                "database",
                "schema",
                "name",
                "equality",
                "inequality",
                "included"
            };

            var boosts = new Dictionary<string, float> {
                { "server", .4F },
                { "database", .3F },
                { "schema", .2F },
                { "name", .1F },
                { "equality", .0F },
                { "inequality", .0F },
                { "included", .0F }
            };

            _directory = FSDirectory.Open(new DirectoryInfo(indexPath));
            _searcher = new IndexSearcher(_directory, true);
            _analyzer = new StandardAnalyzer(Version.LUCENE_30);
            _parser = new MultiFieldQueryParser(Version.LUCENE_30, fields, _analyzer, boosts) {
                DefaultOperator = QueryParser.Operator.AND
            };

            _logger.Trace("Searcher is ready.");
        }

        #region IScriptSearcher Members

        public IEnumerable<IDictionary<string,string>> Search(string q)
        {
            var query = _parser.Parse(q);

            var topFieldCollector = TopFieldCollector.Create(
                sort: Sort.RELEVANCE,
                numHits: 1000,
                fillFields: false,
                trackDocScores: true,
                trackMaxScore: false,
                docsScoredInOrder: false
            );

            _searcher.Search(query, topFieldCollector);

            var topDocs = topFieldCollector.TopDocs();

            if (topDocs == null)
                return new List<IDictionary<string,string>>();

            var missingIndices = topDocs.ScoreDocs.Select(hit => Docs.DocToDict(_searcher.Doc(hit.Doc), hit.Score)).ToArray();
            _logger.Debug("Search of '{0}' return {1} results.", q, missingIndices.Count());
            
            return missingIndices.OrderByDescending(dict=>dict["score"]).ThenByDescending(dict=>dict["rank"]).Take(_resultsLimit);
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
            var keyworldAnalyzer = new KeywordAnalyzer();
            var parser = new QueryParser(Version.LUCENE_30, "id", keyworldAnalyzer);
            var query = parser.Parse(id);
            var scoreDocs = _searcher.Search(query, 1).ScoreDocs;

            if (scoreDocs != null && scoreDocs.Length > 0)
                return Docs.DocToDict(_searcher.Doc(scoreDocs[0].Doc));

            return new Dictionary<string, string>();
        }

        public IEnumerable<IDictionary<string, string>> ReadHighScores(int limit) {
            var results = new List<IDictionary<string, string>>();

            if (_directory != null) {
                try {
                    var reader = IndexReader.Open(_directory, true);
                    var numDocs = reader.NumDocs();
                    for (var i = 0; i < numDocs; i++) {
                        if (reader.IsDeleted(i))
                            continue;
                        results.Add(Docs.DocToDict(reader.Document(i)));
                    }

                    _logger.Debug("Closing");
                    reader.Dispose();
                }
                catch (Exception) {
                    _logger.Info("Attempted to read empty search index.");
                }
            }

            _logger.Info("Found {0}.", results.Count);

            return results.OrderByDescending(dict => dict["score"]).Take(limit);
        }

    }
}