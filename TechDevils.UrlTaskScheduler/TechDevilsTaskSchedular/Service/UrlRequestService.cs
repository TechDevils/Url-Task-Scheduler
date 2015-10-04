using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using log4net;
using TechDevils.UrlTaskScheduler.Models;
using TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service.Interfaces;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service
{
    public class UrlRequestService : IUrlRequestService
    {

        private ILog log;

        public UrlRequestService()
        {
            log = LogManager.GetLogger(typeof (UrlRequestService));
        }

        public int RequestUrl(string url, int id, bool returnResult = false)
        {
            var status = 0;

            try
            {
                //ToDo : Sort http out so that it picks the right url check for relative paths
                var webRequest = (HttpWebRequest) WebRequest.Create("http://"+url);

                var httpWebResponse = webRequest.GetResponse();

                if (returnResult)
                {
                    var responseStream = new StreamReader(httpWebResponse.GetResponseStream());

                    var result = responseStream.ReadToEnd();
                }
                log.Info("Completed url request " + id);

                status = 200;
            }
            catch (WebException we)
            {
                var response = (HttpWebResponse) we.Response;

                status = (int)response.StatusCode;

            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return status;
        }

    }
}