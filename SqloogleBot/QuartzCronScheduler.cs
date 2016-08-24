using Common.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace SqloogleBot {

    public class QuartzCronScheduler {
        readonly IScheduler _scheduler;
        private readonly Options _options;
        private readonly ILog _logger;

        public QuartzCronScheduler(Options options, IJobFactory jobFactory, ILoggerFactoryAdapter loggerFactory) {
            _options = options;
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.JobFactory = jobFactory;

            LogManager.Adapter = loggerFactory;
            _logger = LogManager.GetLogger("Quartz.Net");
        }

        public void Start() {

            _logger.Info($"Starting Scheduler: {_options.Schedule}");
            _scheduler.Start();

            var job = JobBuilder.Create<SqloogleJob>()
                .WithIdentity("Job", "SQLoogleBotJob")
                .StoreDurably(false)
                .RequestRecovery(false)
                .WithDescription("I am a SQLoogleBot Quartz.Net Job!")
                .UsingJobData("Schedule", _options.Schedule)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("Trigger", "SQLoogleBotTrigger")
                .StartNow()
                .WithCronSchedule(_options.Schedule, x => x.WithMisfireHandlingInstructionIgnoreMisfires())
                .Build();

            _scheduler.ScheduleJob(job, trigger);
        }

        public void Stop() {
            if (!_scheduler.IsStarted)
                return;

            _logger.Info("Stopping Scheduler...");
            _scheduler.Shutdown(true);
        }

    }
}
