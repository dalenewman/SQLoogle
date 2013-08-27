using System.Configuration;
using System.IO;
using Sqloogle.Operations;
using Sqloogle.Operations.Support;

namespace Sqloogle.Processes {

    /// <summary>
    /// The SQLoogle process.  This is where it all comes together.  It SQLoogles!
    /// </summary>
    public class SqloogleProcess : SqloogleEtlProcess {

        protected override void Initialize() {

            var config = (SqloogleBotConfiguration)ConfigurationManager.GetSection("sqloogleBot");

            if (!Directory.Exists(config.SearchIndexPath))
                Directory.CreateDirectory(config.SearchIndexPath);

            Register(new ParallelUnionAllOperation(config.ServerCrawlProcesses()));
            Register(new SqloogleAggregate());
            Register(new SqloogleTransform());
            Register(new SqloogleCompare().Right(new LuceneExtract(config.SearchIndexPath)));
            RegisterLast(new LuceneLoad(config.SearchIndexPath));
        }
    }
}
