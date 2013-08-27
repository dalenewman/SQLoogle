using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Operations;

namespace Sqloogle.Processes {

    public class ReportingServicesProcess : PartialProcessOperation {

        public ReportingServicesProcess(string connectionString) {
            Register(new DatabaseExtract(connectionString));
            Register(new ReportingServicesExtract());
            Register(new ReportingServicesTransform());
        }
    }
}
