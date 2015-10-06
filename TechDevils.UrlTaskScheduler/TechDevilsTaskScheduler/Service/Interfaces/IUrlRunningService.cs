using System;
using System.Collections.Generic;
using TechDevils.UrlTaskScheduler.Models;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskScheduler.Service.Interfaces
{
    public interface IUrlRunningService
    {
        void CallUrls(List<ScheduleUrl> urls);
        int CallUrl(ScheduleUrl url);
        void SetNextRun(ScheduleUrl url);
        string GetAndRunUrls(List<ScheduleUrl> urls);

        //void SaveRecord(int id, string url, int requestStatus, string returnMessage = null);
    }
}