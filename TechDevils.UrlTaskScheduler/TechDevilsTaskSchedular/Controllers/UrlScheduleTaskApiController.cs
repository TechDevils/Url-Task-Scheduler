using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using log4net;
using TechDevils.UrlTaskScheduler.Models;
using TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service;
using Umbraco.Core.Persistence;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Controllers
{
    //ToDo : Create dashborad features

    [PluginController("TaskScheduler")]
    public class UrlScheduleTaskApiController : UmbracoAuthorizedJsonController
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<ScheduleUrl> GetAll()
        {
            var query = new Sql().Select("*").From("TD_ScheduleUrl");
            return DatabaseContext.Database.Fetch<ScheduleUrl>(query);
        }

        public ScheduleUrl GetTaskById(int id)
        {
            var query = new Sql().Select("*").From("TD_ScheduleUrl").Where("id =" + id);

            var result = DatabaseContext.Database.Fetch<ScheduleUrl>(query).FirstOrDefault();

            return result;
        }

        public ScheduleUrl PostSave(ScheduleUrl url)
        {
            var ts = new TimingService();

            url.NextRun = ts.GetNextRun(url); ;
            url.StartFrom = DateTime.UtcNow;

            url.LastFiveOutcomes = "x,x,x,x,x";

            if (url.Id > 0)
                DatabaseContext.Database.Update(url);
            else
                DatabaseContext.Database.Save(url);

            return url;
        }

        public int DeleteById(int id)
        {
            return DatabaseContext.Database.Delete<ScheduleUrl>(id);
        }

        public string RunTaskNow(int id)
        {
            //ToDo: Get run now task to update the last manual run and not to update the nex run time
            var sql = new Sql().Select("*").From<ScheduleUrl>().Where("id = " + id);

            var records = DatabaseContext.Database.Fetch<ScheduleUrl>(sql);

            var rs = new UrlRequestService();
            var urlRunningService = new UrlRunningService(rs);

            try
            {
                urlRunningService.CallUrls(records);
            }
            catch (Exception e)
            {
                _log.Error("Error running 'Run Now' task",e);
                return "Failed to run please see log for more information";
            }

            return DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss.fff");
        }

        public bool DisableUrl(int id)
        {
            if (umbraco.helper.GetCurrentUmbracoUser().IsAdmin())
            {
                SetDisableValue(id, true);

                return true;
            }
            return false;
        }

        public bool EnableUrl(int id)
        {
            if (umbraco.helper.GetCurrentUmbracoUser().IsAdmin())
            {
                SetDisableValue(id, false);

                return true;
            }
           return false;
        }

        //ToDo : Reset status

        [HttpPost]
        public List<ScheduleUrl> GetStatus(int page = 1, int pageSize = 10)
        {
            if (umbraco.helper.GetCurrentUmbracoUser().IsAdmin())
            {
                var query = new Sql().Select("*").From("TD_ScheduleUrl");

                var db = ApplicationContext.DatabaseContext.Database;

                var urls = db.Fetch<ScheduleUrl>(query);

                urls = urls.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return urls;
            }

            return null;
        }


        private void SetDisableValue(int id, bool value)
        {
            var db = DatabaseContext.Database;

            var record = db.SingleOrDefault<ScheduleUrl>("SELECT * FROM TD_ScheduleUrl WHERE Id = @0", id);

            record.Disabled = value;

            db.Update("TD_ScheduleUrl", "Id", record);
        }

    }
}