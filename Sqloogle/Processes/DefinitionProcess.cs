using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Operations;

namespace Sqloogle.Processes {

    /// <summary>
    /// A DefinitionProcess queries object definitions from each database
    /// on a server.  It also queries statistics (use and last used).
    /// </summary>
    public class DefinitionProcess : PartialProcessOperation {

        public DefinitionProcess(string connectionString) {
            Register(new DatabaseExtract(connectionString));
            Register(new DatabaseFilter());
            Register(new DefinitionExtract());
            Register(new CachedObjectStatsJoin().Right(new CachedObjectStatsExtract(connectionString)));
            Register(new TableStatsJoin().Right(new TableStatsExtract(connectionString)));
            RegisterLast(new IndexStatsJoin().Right(new IndexStatsExtract(connectionString)));
        }
    }
}
