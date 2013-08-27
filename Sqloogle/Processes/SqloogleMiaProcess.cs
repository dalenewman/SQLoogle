using System.Configuration;
using System.IO;
using Sqloogle.Operations;
using Sqloogle.Operations.Support;

namespace Sqloogle.Processes {

    /// <summary>
    /// The SQLoogle MIA (Missing Index Analyzer) process.
    /// </summary>
    public class SqloogleMiaProcess : SqloogleEtlProcess {

        protected override void Initialize() {

            var config = (SqloogleBotConfiguration)ConfigurationManager.GetSection("sqloogleBot");
            var miaFolder = Path.Combine(config.SearchIndexPath, "MIA");

            if (!Directory.Exists(miaFolder))
                Directory.CreateDirectory(miaFolder);

            using (var writer = new LuceneWriter(miaFolder)) {
                writer.Clean();
            }

            Register(new ParallelUnionAllOperation(config.ServerMiaCrawlProcesses()));
            Register(new MissingIndexTransform());
            RegisterLast(new LuceneLoad(miaFolder));
        }
    }
}
