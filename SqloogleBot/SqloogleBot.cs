using Sqloogle.Processes;

namespace SqloogleBot {
    class SqloogleBot {
        static void Main(string[] args) {

            var sqloogle = new SqloogleProcess();
            sqloogle.Execute();
            sqloogle.ReportErrors();


            var sqloogleMia = new SqloogleMiaProcess();
            sqloogleMia.Execute();
            sqloogleMia.ReportErrors();

        }
    }
}