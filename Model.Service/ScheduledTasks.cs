using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Service
{
    public class ScheduledTasks
    {
        public static void Start()
        {
            try
            {
                var jobKey = new JobKey("jobQuartzBirthday", "jobQuartzBirthday");
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Clear();
                if (!scheduler.CheckExists(jobKey))
                {
                    IJobDetail job = JobBuilder.Create<HeThongThongBao>().WithIdentity(jobKey).Build();

                    ITrigger trigger = TriggerBuilder.Create()
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 2))
                      )
                    .Build();
                    scheduler.ScheduleJob(job, trigger);
                    scheduler.Start();
                }
            }
            catch
            {

            }
        }

        public static void DatLich()
        {
            try
            {
                var jobKey = new JobKey("jobQuartzBirthday", "jobQuartzBirthday");
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Clear();
                    IJobDetail job = JobBuilder.Create<LichHenJob>().WithIdentity(jobKey).Build();
                    ITrigger trigger = TriggerBuilder.Create()
                               .WithDescription("Once")
                               .WithSimpleSchedule(x => x
                               .WithIntervalInSeconds(40)
                               .RepeatForever())
                               .StartAt(DateBuilder.DateOf(16, 57, 00, 23, 01, 2019))
                               .EndAt(DateBuilder.DateOf(16, 60, 00, 23, 01, 2019))
                               .Build();
                    scheduler.ScheduleJob(job, trigger);
                    scheduler.Start();
                
            }
            catch
            {

            }
        }
    }
}
