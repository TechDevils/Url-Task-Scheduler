using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.businesslogic;
using umbraco.interfaces;

namespace TechDevils.UrlTaskScheduler.Sections
{
    [Application("scheduleUrlsArea", "Url Scheduler", "icon-time", 10)]
    public class ScheduleUrlArea : IApplication {}
}
