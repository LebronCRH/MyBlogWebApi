using Autofac;
using MyBlog.Core.Web.Jobs.Core;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Core.Web.Jobs
{
    public class QuartzRegister
    {
        public static void UseQuartz(IContainer container)
        {
            JobManager.Scheduler = container.Resolve<IScheduler>();
            JobManager.Scheduler.Start();

            // 测试定时TestJob
            //JobManager.AddSchedule(new JobServer<TestJob>(),
            //    new TestTriggerServer().TestTrigger(), "Test", "group");

        }
    }
}
