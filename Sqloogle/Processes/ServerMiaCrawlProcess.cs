using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Operations;

namespace Sqloogle.Processes {

    public class ServerMiaCrawlProcess : PartialProcessOperation {

        public ServerMiaCrawlProcess(string connectionString, string server) {
            Register(new DatabaseExtract(connectionString));
            Register(new DatabaseFilter());
            RegisterLast(new MissingIndexExtract(server));
        }
    }
}
