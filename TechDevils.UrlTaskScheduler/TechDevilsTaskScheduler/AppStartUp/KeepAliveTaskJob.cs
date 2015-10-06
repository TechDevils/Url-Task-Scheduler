using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using log4net;
using Quartz;
using umbraco.BusinessLogic;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskScheduler.AppStartUp
{
    public class KeepAliveTaskJob : IJob
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            var url = context.JobDetail.JobDataMap["KeepAliveUrl"];
            _log.Info("Started keep alive task !!");
            _log.Info(url);

            var webRequest = (HttpWebRequest)WebRequest.Create(url.ToString());

            webRequest.Method = "GET";
            webRequest.ContentType = "text/html";
            webRequest.Proxy = new WebProxy();

            try
            {
                var response = webRequest.GetResponse();
                _log.Info("Got response");
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;

                _log.Error("Got response :" + response.StatusCode,e);
            }
        }
    }
}