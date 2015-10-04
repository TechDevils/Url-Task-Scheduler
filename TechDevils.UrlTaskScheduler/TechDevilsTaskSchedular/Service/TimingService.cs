using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechDevils.UrlTaskScheduler.Models;
using TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service.Interfaces;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service
{
    public class TimingService : ITimingService
    {
        public DateTime GetNextRun(ScheduleUrl record)
        {
            var nextRun = DateTime.UtcNow;

            switch (record.RunningType)
            {
                case "interval":
                    nextRun = DateTime.UtcNow.AddMinutes(record.MinuteInterval);
                    break;
                case "dayAndTime":
                    var nextDate = DateTime.UtcNow;
                    
                    int hour;
                    int min;

                    var timeSplit = record.TimeToRun.Split(':');

                    int.TryParse(timeSplit[0], out hour);
                    int.TryParse(timeSplit[1], out min);

                    if (hour <= nextDate.Hour && min < nextDate.Minute)
                    {
                        nextDate = nextDate.AddDays(1);
                    }

                    var selectedDays = record.DaysToRun.Split(';');

                    var daysToRun = selectedDays.Where(x => (x.IndexOf(":1") > -1));

                    if (daysToRun.Any())
                    {
                        var foundDate = false;
                        do
                        {
                            if (
                                daysToRun.Any(
                                    x => x.IndexOf(nextDate.DayOfWeek.ToString().ToLower().Substring(0, 3)) > -1))
                            {
                                foundDate = true;
                            }
                            else
                            {
                                nextDate = nextDate.AddDays(1);
                            }
                        } while (!foundDate);
                    }
                    else
                    {
                        return default(DateTime);
                    }

                    var time = new TimeSpan(hour, min, 00);

                    nextRun = nextDate.Date + time;

                    if (!nextRun.IsDaylightSavingTime())
                    {
                        nextRun = nextRun.AddHours(-1);
                    }

                    break;
            }

            return nextRun;
        }
    }
}