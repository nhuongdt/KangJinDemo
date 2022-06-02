using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace banhang24.Hellper
{
    public class ScheduledTasks

    {

        public static void Start()

        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();



            IJobDetail job = JobBuilder.Create<HeThongThongBao>().Build();



            ITrigger trigger = TriggerBuilder.Create()

                .WithDailyTimeIntervalSchedule

                  (s =>

                     s.WithIntervalInMinutes(1)

                    .OnEveryDay()

                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(15, 36))

                  )

                .Build();



            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();

        }

    }
}