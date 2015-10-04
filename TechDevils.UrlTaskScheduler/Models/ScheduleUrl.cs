using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Threading;
using System.Web;
using TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Enums;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Umbraco.Web.PropertyEditors;

namespace TechDevils.UrlTaskScheduler.Models
{
    [TableName("TD_ScheduleUrl")]
    [DataContract(Name = "scheduleUrl")]
    public class ScheduleUrl
    {
        public ScheduleUrl()
        {
        }

        [PrimaryKeyColumn(AutoIncrement = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }
        // ng-pattern="(?:(?:https?|ftp)://)(?:\S+(?::\S*)?@)?(?:(?!10(?:\.\d{1,3}){3})(?!127(?:\.\d{1,3}){3})(?!169\.254(?:\.\d{1,3}){2})(?!192\.168(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\x{00a1}-\x{ffff}0-9]+-?)*[a-z\x{00a1}-\x{ffff}0-9]+)(?:\.(?:[a-z\x{00a1}-\x{ffff}0-9]+-?)*[a-z\x{00a1}-\x{ffff}0-9]+)*(?:\.(?:[a-z\x{00a1}-\x{ffff}]{2,})))(?::\d{2,5})?(?:/[^\s]*)?"
        //ToDo : Add regex validation
        [DataMember(Name = "url")]
        [Length(2083)]
        public string Url { get; set; }

        [DataMember(Name = "desc")]
        public string Description { get; set; }

        [DataMember(Name = "runningType")]
        public string RunningType { get; set; }
        [DataMember(Name = "isHttps")]
        public bool IsHttps { get; set; }
        [DataMember(Name = "lastManualRun")]
        [NullSetting]
        public DateTime? LastManualRun { get; set; }
        [DataMember(Name = "lastRun")]
        [NullSetting]
        public DateTime LastRun { get; set; }

        [DataMember(Name = "startNextRunFrom")]
        public DateTime StartFrom { get; set; }

        [DataMember(Name = "nextRun")]
        [NullSetting]
        public DateTime NextRun { get; set; }
        [DataMember(Name = "daysToRunOutput")]
        public string DaysToRun { get; set; }
        [DataMember(Name = "minuteInterval")]
        public int MinuteInterval { get; set; }
        [DataMember(Name = "timeToRun")]
        public string TimeToRun { get; set; }

        [DataMember(Name = "disabled")]
        public bool Disabled { get; set; }

        [DataMember(Name = "currentTaskStatus")]
        public byte UrlTaskStatusValue { get; set; }

        [Ignore]
        public UrlTaskStatus UrlTaskStatus { get { return (UrlTaskStatus)UrlTaskStatusValue; } }

        [DataMember(Name = "lastFiveOutcomes")]
        public string LastFiveOutcomes { get; set; }

        //ToDo : Add status of last five runs
        //ToDo : Add notify email address
        //ToDo : Add Response in email report

    }
}