using System.Collections.Generic;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service.Interfaces
{
    public interface IUrlRequestService
    {
        int RequestUrl(string url, int id, bool returnResult = false);
    }
}