using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using log4net;
using TechDevils.UrlTaskScheduler.Models;
using TechDevils.UrlTaskScheduler.TechDevilsTaskScheduler.Service;
using TechDevils.UrlTaskScheduler.TechDevilsTaskScheduler.Service.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskScheduler.Controllers
{
    public class UrlTaskRunnerController : UmbracoApiController
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IUrlRunningService _urlRunningService { get; set; }
        private IUrlRequestService _urlRequestService { get; set; }

        public UrlTaskRunnerController()
        {
            if (_urlRequestService == null)
            {
                _urlRequestService = new UrlRequestService();
            }
            _urlRunningService = new UrlRunningService(_urlRequestService);
        }

        [System.Web.Http.AcceptVerbs("GET")]
        public string UrlRunner()
        {
             var query = new Sql()
                .Select("*")
                .From("TD_ScheduleUrl")
                //.Where("LastRun < @0", DateTime.UtcNow.AddSeconds(-6.5))
                //.Where("NextRun > @0", DateTime.UtcNow)
                //.Where("NextRun < @0", DateTime.UtcNow.AddMinutes(1))
                .Where("Disabled = @0", false);

            var urls = DatabaseContext.Database.Fetch<ScheduleUrl>(query);

            return _urlRunningService.GetAndRunUrls(urls);
        }
    }
}