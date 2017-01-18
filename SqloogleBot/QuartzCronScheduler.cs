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
