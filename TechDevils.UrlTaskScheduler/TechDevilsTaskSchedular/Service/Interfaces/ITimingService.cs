using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechDevils.UrlTaskScheduler.Models;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service.Interfaces
{
    public interface ITimingService
    {
        DateTime GetNextRun(ScheduleUrl url);
    }
}
