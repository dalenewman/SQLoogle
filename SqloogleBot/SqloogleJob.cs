using Quartz;
using Sqloogle.Processes;

namespace SqloogleBot {
    public class SqloogleJob : IJob {
        public void Execute(IJobExecutionContext context) {
            using (var sqloogle = new SqloogleProcess()) {
                sqloogle.Execute();
                sqloogle.ReportErrors();
            }

            using (var sqloogleMia = new SqloogleMiaProcess()) {
                sqloogleMia.Execute();
                sqloogleMia.ReportErrors();
            }

        }

    }
}