using NUnit.Framework;
using Sqloogle.Processes;

namespace Tests.Integration {
    [TestFixture]
    public class RunSqloogleMiaProcess {

        [Test]
        public void Run() {
            var process = new SqloogleMiaProcess();
            process.Execute();
            process.ReportErrors();
        }

    }
}
