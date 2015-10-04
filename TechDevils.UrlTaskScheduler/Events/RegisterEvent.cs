using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechDevils.UrlTaskScheduler.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace TechDevils.UrlTaskScheduler.Events
{
    public class RegisterEvent : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var db = applicationContext.DatabaseContext.Database;

            //Check Table does NOT Exists
            if (!db.TableExist("TD_ScheduleUrl"))
            {
                db.CreateTable<ScheduleUrl>(false);
            }
        }
    }
}