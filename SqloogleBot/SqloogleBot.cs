#region license
// Sqloogle
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
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