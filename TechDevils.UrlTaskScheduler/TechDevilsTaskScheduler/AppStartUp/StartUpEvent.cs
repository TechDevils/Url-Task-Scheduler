using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Web;
using log4net;
using Quartz;
using Quartz.Impl;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskScheduler.AppStartUp
{
    public class StartUpEvent : IApplicationEventHandler
    {
        public ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private void StartQuartzScheduler(object sender, EventArgs e)
        {
            _log.Info("Starting Quartz Scheduler");
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            var umrbacoScheduledTask = UmbracoConfig.For.UmbracoSettings().ScheduledTasks;

            var baseUrl = UmbracoConfig.For.UmbracoSettings().Help.DefaultUrl ; 
            
            var url = "";

            if (umrbacoScheduledTask != null 
                && umrbacoScheduledTask.Tasks.Any(x => x.Alias == "TechDevilsScheduledTask"))
            {
                var task = umrbacoScheduledTask.Tasks.FirstOrDefault(x => x.Alias == "TechDevilsScheduledTask");
                url = task.Url;
            }

            IJobDetail job = JobBuilder.Create<UrlTaskJob>().UsingJobData("pingUrl",url).Build();

            //var trigger = TriggerBuilder.Create().WithCronSchedule("* 1 * * * ? *").Build();
            var trigger = TriggerBuilder.Create().WithIdentity("MinTask").StartNow().WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()).Build();

            if (!scheduler.CheckExists(trigger.Key) && !string.IsNullOrEmpty(url))
            {
                scheduler.ScheduleJob(job, trigger);
                _log.Info("Triggered Quartz Scheduler");
            }
            else
            {
                if (scheduler.CheckExists(trigger.Key))
                {
                    _log.Info("Not Triggered Quartz Scheduler as Trigger already exists");
                }
                else
                {
                    _log.Info("Not Triggered Quartz Scheduler as Url is empty");
                }
            }
            
        }

        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            _log.Info("Pre app start task");
            UmbracoApplicationBase.ApplicationInit += StartQuartzScheduler;            
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            UmbracoApplicationBase.ApplicationStarted +=UmbracoApplicationBaseOnApplicationStarted;
        }

        private void UmbracoApplicationBaseOnApplicationStarted(object sender, EventArgs eventArgs)
        {
            var url = ConfigurationManager.AppSettings["KeepAliveUrl"];
            if (!string.IsNullOrEmpty(url))
            {
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                var job = JobBuilder.Create<KeepAliveTaskJob>().UsingJobData("KeepAliveUrl", url).Build();
                var trigger =
                    TriggerBuilder.Create()
                        .WithIdentity("KeepAliveTask")
                        .StartNow()
                        .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
                        .Build();
                if (!scheduler.CheckExists(trigger.Key) && !string.IsNullOrEmpty(url))
                {
                    scheduler.ScheduleJob(job, trigger);
                    _log.Info("Triggered Quartz Scheduler");
                }
                else
                {
                    if (scheduler.CheckExists(trigger.Key))
                    {
                        _log.Info("Not Triggered Quartz Scheduler as Trigger already exists");
                    }
                    else
                    {
                        _log.Info("Not Triggered Quartz Scheduler as Url is empty");
                    }
                }
            }
        }
    }
}