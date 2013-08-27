using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Operations;
using Sqloogle.Operations.Support;

namespace Sqloogle.Processes {

    /// <summary>
    /// A ServerProcess queries cached sql, sql agent jobs,
    /// reporting services commands, and object definitions from
    /// a single SQL Server. 
    /// </summary>
    public class ServerCrawlProcess : PartialProcessOperation {

        public ServerCrawlProcess(string connectionString, string server) {

            var union = new ParallelUnionAllOperation(
                new DefinitionProcess(connectionString),
                new CachedSqlProcess(connectionString),
                new SqlAgentJobExtract(connectionString),
                new ReportingServicesProcess(connectionString)
            );

            Register(union);
            RegisterLast(new AppendToRowOperation("server", server));

        }
    }
}
