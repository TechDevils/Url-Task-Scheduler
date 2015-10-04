using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using log4net;
using log4net.Util;
using TechDevils.UrlTaskScheduler.Models;
using TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Enums;
using TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service
{
    public class UrlRunningService : IUrlRunningService
    {

        private IUrlRequestService _requestService;

        private readonly SaveUrlStatus _saveResult;
        private readonly EmailFinalResult _emailFinalResult;
        private readonly UrlPostStatusTask _completeStatus;
        private readonly UrlPostStatusTask _saveTasks;

        private readonly TimingService _timingService;
        private UmbracoDatabase _database;
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void SaveUrlStatus(int id, string url, int requestStatus, string resultMessage = null);
        public delegate void EmailFinalResult(List<ScheduleUrl> records);
        public delegate void UrlPostStatusTask(ScheduleUrl record, Task<int> resultOfTask);
        public delegate void UrlPreRunStatusTask(ScheduleUrl record);

        public ILog Log { get; set; }

        public UrlRunningService(IUrlRequestService requestService)//, EmailFinalResult emailFinalResult)
        {
            _database = new UmbracoDatabase("umbracoDbDSN");

            _timingService = new TimingService();
            _requestService = requestService;
            Log = LogManager.GetLogger(typeof (UrlRunningService));
            //_emailFinalResult = emailFinalResult;
            //_completeStatus += SetCurrentStatus;
            //_completeStatus += UpdateLastFiveStatus;
            _completeStatus += SaveTaskInformation;

            _saveTasks += SetCurrentStatus;
            _saveTasks += UpdateLastFiveStatus;
            _saveTasks += SetLastRunTime;
        }

        public string GetAndRunUrls(List<ScheduleUrl> urls)
        {
            var runTime = DateTime.UtcNow.Date + new TimeSpan(DateTime.UtcNow.Hour, DateTime.UtcNow.Minute + 1, 0);

            urls = urls.Where(x => x.NextRun < runTime && !x.Disabled && x.UrlTaskStatus == UrlTaskStatus.inactive).ToList();
            try
            {
                this.CallUrls(urls);
            }
            catch (Exception e)
            {
                _log.Error("Error running 'Task Runner' task", e);
                return "Failed to run please see log for more information";
            }
            //ToDo: Create task to run urls
            //ToDo: Return Run count. count ran and failed

            foreach (var scheduleUrl in urls)
            {
                _log.Info("Ran " + scheduleUrl.Url);
            }

            return "Ran " + urls.Count + ". See logs for more details";
        }

        public void CallUrls(List<ScheduleUrl> urls)
        {
            foreach (var url in urls)
            {
                Log.Info("Started to run : " + url.Url);
                Task.Run(() => CallUrl(url)).ContinueWith((taskResult) => _completeStatus(url, taskResult), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent);
            }
        }

        public int CallUrl(ScheduleUrl url)
        {
            SetNextRun(url);
            SetRunningStatus(url);

            _database.Update(url);

            return _requestService.RequestUrl(url.Url, url.Id);
        }

        private void SetRunningStatus(ScheduleUrl record)
        {
            record.UrlTaskStatusValue = (int)UrlTaskStatus.running;
        }

        public void SetNextRun(ScheduleUrl record)
        {
            record.NextRun = _timingService.GetNextRun(record);
        }


        public void SaveTaskInformation(ScheduleUrl record, Task<int> taskResult)
        {
            var completionTask = Task.Run(() => _saveTasks(record, taskResult));

            Task.WaitAll(completionTask);

            //var query = new Sql().Select("*").From("TD_ScheduleUrl").Where("id =" + record.Id);

            //var result = _database.Fetch<ScheduleUrl>(query).FirstOrDefault();

            _database.Update(record);
        }

        public void SetLastRunTime(ScheduleUrl record, Task<int> taskResult)
        {
            record.LastRun = DateTime.UtcNow;
        }

        public void SetCurrentStatus(ScheduleUrl record, Task<int> taskResult)
        {
            record.UrlTaskStatusValue = (int)UrlTaskStatus.inactive;
        }

        public void UpdateLastFiveStatus(ScheduleUrl record, Task<int> taskResult)
        {
            switch (taskResult.Result)
            {
                case 200:
                    Log.Info("UrlTask complete : " + record.Url);
                    break;
                default:
                    Log.Warn("UrlTask returned status : " + taskResult.Result + " for " + record.Url);
                    break;
            }

            var currentStatus = !string.IsNullOrEmpty(record.LastFiveOutcomes) 
                ? record.LastFiveOutcomes.Split(',') : new [] { "X", "X", "X", "X", "X" };

            var newStatus = new string[5];

            for (int i = 0; i < newStatus.Length; i++)
            {
                if (i == 0)
                {
                    newStatus[i] = taskResult.Result.ToString();
                }
                else
                {
                    try
                    {
                        newStatus[i] = currentStatus[i - 1];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Log.Error("Failed to parse current status. Reset in status into correct format",e);

                        newStatus[i] = "X";
                    }
                }
            }

            var returnStatus = new StringBuilder();

            var first = true;
            foreach (var newStatu in newStatus)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    returnStatus.Append(",");
                }

                returnStatus.Append(newStatu);
            }

            record.LastFiveOutcomes = returnStatus.ToString();

        }

    }
}