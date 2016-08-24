using System;
using System.Threading;
using Sqloogle.Processes;

namespace SqloogleBot {

    class SqloogleBot {

        private const int Wtf = 1;
        static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        static void Main(string[] args) {

            Console.CancelKeyPress += (sender, eArgs) => {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            var options = new Options();

            if (CommandLine.Parser.Default.ParseArguments(args, options)) {

                if (string.IsNullOrEmpty(options.Schedule)) {
                    using (var sqloogle = new SqloogleProcess()) {
                        sqloogle.Execute();
                        sqloogle.ReportErrors();
                    }

                    using (var sqloogleMia = new SqloogleMiaProcess()) {
                        sqloogleMia.Execute();
                        sqloogleMia.ReportErrors();
                    }
                } else {

                    var scheduler = new QuartzCronScheduler(
                        options, 
                        new QuartzJobFactory(), 
                        new QuartzLogAdaptor(Utility.GetConsoleLogLevel(), true, true, false, "o")
                    );
                    Console.WriteLine("Starting SqloogleBot... :-)");
                    Console.WriteLine("Press CTRL-C to stop.");
                    scheduler.Start();

                    QuitEvent.WaitOne();
                    Console.WriteLine("Stopping SqloogleBot... :-(");
                    scheduler.Stop();
                }
            } else {
                Environment.ExitCode = Wtf;
            }


        }
    }
}