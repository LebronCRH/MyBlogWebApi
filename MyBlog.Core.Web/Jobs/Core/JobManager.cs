using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Core.Web.Jobs.Core
{
    public class JobManager
    {

        private static IScheduler _scheduler;
        public static IScheduler Scheduler
        {
            get
            {
                return _scheduler;
            }
            set { _scheduler = value; }

        }

        public static void AddSchedule<T>(JobServer<T> jobServer, ITrigger trigger, string jobName, string jobGroup) where T : IJob
        {
            jobServer.JobName = jobName;
            jobServer.JobGroup = jobGroup;
            _scheduler.ScheduleJob(jobServer.CrateJob(), trigger);
        }
    }
}
