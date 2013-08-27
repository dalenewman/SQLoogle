using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Operations;

namespace Sqloogle.Processes {
    public class CachedSqlProcess : PartialProcessOperation {
        public CachedSqlProcess(string connectionString) {
            Register(new CachedSqlExtract(connectionString));
            Register(new CachedSqlPreTransform());
            Register(new CachedSqlAggregate());
            Register(new CachedSqlPostTransform());
        }
    }
}
