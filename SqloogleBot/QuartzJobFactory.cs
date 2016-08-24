using System;
using Quartz;
using Quartz.Spi;
using IScheduler = Quartz.IScheduler;

namespace SqloogleBot {

    public class QuartzJobFactory : IJobFactory {

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) {
            return null;
        }

        public void ReturnJob(IJob job) {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }

}
