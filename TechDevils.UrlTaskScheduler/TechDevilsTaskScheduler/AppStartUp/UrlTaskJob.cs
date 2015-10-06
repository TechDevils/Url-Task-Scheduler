using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using ClientDependency.Core;
using log4net;
using Quartz;
using TechDevils.UrlTaskScheduler.Models;
using TechDevils.UrlTaskScheduler.TechDevilsTaskScheduler.Service;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskScheduler.AppStartUp
{
    public class UrlTaskJob :IJob
    {
        //private readonly IUrlRunningService _runningService;

        //public UrlTaskJob(IUrlRunningService runningService)
        //{
        //    _runningService = runningService;
        //}

        public ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {
            //var mapper = context.JobDetail.JobDataMap;
            //var url = mapper.GetString("pingUrl");

            //var request = (HttpWebRequest)WebRequest.Create(url);

            //request.ContentLength = 0;
            //request.Headers.Add("Accept-Language", "en;q=0.8");
            //request.ContentType = "text/html";
            //request.Method = "GET";

            //try
            //{
            //    var response = (HttpWebResponse)request.GetResponse();
            //    _log.Info("Ran Quartz UrlJob : " + url);
            //}
            //catch (WebException e)
            //{
            //    var response = (HttpWebResponse)e.Response;

            //    _log.Error("Failed Quartz UrlJob : " + url + " Status :" + response.StatusCode, e);
            //}
            var requestService = new UrlRequestService();
            var runningService = new UrlRunningService(requestService);

            var query = new Sql()
                .Select("*")
                .From("TD_ScheduleUrl")
                                //.Where("LastRun < @0", DateTime.UtcNow.AddSeconds(-6.5))
                                //.Where("NextRun > @0", DateTime.UtcNow)
                                //.Where("NextRun < @0", DateTime.UtcNow.AddMinutes(1))
                .Where("Disabled = @0", false);

            var urls = ApplicationContext.Current.DatabaseContext.Database.Fetch<ScheduleUrl>(query);
            try
            {
                var response = runningService.GetAndRunUrls(urls);
                _log.Info("Ran TechDevils Job : " + response);
            }
            catch (Exception e)
            {
                _log.Error("Failed TechDevils Job", e);
            }

            
        }
    }
}