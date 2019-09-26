using System.Collections.Generic;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskScheduler.Service.Interfaces
{
    public interface IUrlRequestService
    {
        int RequestUrl(bool isHttps, string url, int id, bool returnResult = false);
    }
}