using CommandLine;
using CommandLine.Text;

namespace SqloogleBot {
    public class Options {
        [Option('s', "schedule", Required = false, HelpText = "a cron expression (http://www.quartz-scheduler.org/documentation/quartz-1.x/tutorials/crontrigger)")]
        public string Schedule { get; set; }

        [HelpOption]
        public string GetUsage() {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

    }
}
