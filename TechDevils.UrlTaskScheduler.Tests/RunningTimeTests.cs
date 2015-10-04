using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TechDevils.UrlTaskScheduler.Models;
using TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service;
using TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Service.Interfaces;

namespace TechDevils.UrlTaskScheduler.Tests
{
    [TestFixture]
    public class RunningTimeTests
    {

        private ITimingService _timingService { get; set; }

        [SetUp]
        public void Setup()
        {
            _timingService = new TimingService();
        }

        [Test]
        public void NextRunInterval5mins()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 5;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "interval";

            var restult = _timingService.GetNextRun(model);

            var totalTimeGap = restult - model.LastRun;

            Assert.AreEqual(5, totalTimeGap.TotalMinutes);

        }

        [Test]
        public void NextRunInterval10mins()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 10;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "interval";

            var restult = _timingService.GetNextRun(model);

            var totalTimeGap = restult - model.LastRun;

            Assert.AreEqual(10, totalTimeGap.TotalMinutes);

        }

        [Test]
        public void DiffBetweenNextAndLastRunIs12Mins()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 12;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "interval";

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("HH:mm");

            var lastRunAdd12Min = model.LastRun.AddMinutes(12).ToString("HH:mm");

            Assert.AreEqual(lastRunAdd12Min, nextRunTime);

        }

        [Test]
        public void DiffBetweenNextAndLastRunBasedOnTimeAndDate2days()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 12;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "interval";

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("HH:mm");

            var lastRunAdd12Min = model.LastRun.AddMinutes(12).ToString("HH:mm");

            Assert.AreEqual(lastRunAdd12Min, nextRunTime);

        }

        [Test]
        public void NextRunIsMondayAt12()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 0;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "dayAndTime";
            model.DaysToRun = "mon:1;tue:0;wed:0;thu:0;fri:0;sat:0;sun:0";
            model.TimeToRun = "12:00";

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("dddhhmm");

            Assert.AreEqual("Mon1200", nextRunTime);

        }

        [Test]
        public void NextRunIsTuesdayAt12()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 0;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "dayAndTime";
            model.DaysToRun = "mon:0;tue:1;wed:0;thu:0;fri:0;sat:0;sun:0";
            model.TimeToRun = "12:00";

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("dddhhmm");

            Assert.AreEqual("Tue1200", nextRunTime);

        }

        [Test]
        public void NextRunIsSundayAt12()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 0;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "dayAndTime";
            model.DaysToRun = "mon:0;tue:0;wed:0;thu:0;fri:0;sat:0;sun:1";
            model.TimeToRun = "12:00";

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("dddhhmm");

            Assert.AreEqual("Sun1200", nextRunTime);

        }

        [Test]
        public void NextRunIsNextSaturdayAt12()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 0;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "dayAndTime";
            model.DaysToRun = "mon:0;tue:0;wed:0;thu:0;fri:0;sat:1;sun:0";
            model.TimeToRun = "12:00";

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("dd:dddhhmm");

            var nextSaturday = DateTime.UtcNow;

            while (nextSaturday.DayOfWeek != DayOfWeek.Saturday)
            {
                nextSaturday = nextSaturday.AddDays(1);
            }

            var twleve = new TimeSpan(12, 00, 00);
            nextSaturday = nextSaturday.Date + twleve;

            Assert.AreEqual(nextSaturday.ToString("dd:dddhhmm"), nextRunTime);

        }

        [Test]
        public void IfAllValuesOnDaysToRunAre0GetNextRunToDefault()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 0;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "dayAndTime";
            model.DaysToRun = "mon:0;tue:0;wed:0;thu:0;fri:0;sat:0;sun:0";
            model.TimeToRun = "12:00";

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result;

            Assert.AreEqual(default(DateTime), nextRunTime);

        }

        [Test]
        public void NextRunIsNextSaturdayAt1438()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 0;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "dayAndTime";
            model.DaysToRun = "mon:0;tue:0;wed:0;thu:0;fri:0;sat:1;sun:0";
            model.TimeToRun = "14:38";

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("dd:dddHHmm");

            var nextSaturday = DateTime.UtcNow;

            while (nextSaturday.DayOfWeek != DayOfWeek.Saturday)
            {
                nextSaturday = nextSaturday.AddDays(1);
            }

            var twleve = new TimeSpan(14, 38, 00);
            nextSaturday = nextSaturday.Date + twleve;

            Assert.AreEqual(nextSaturday.ToString("dd:dddHHmm"), nextRunTime);

        }

        [Test]
        public void NextRunIsNextMonWedSatAt1438()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 0;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "dayAndTime";
            model.DaysToRun = "mon:1;tue:0;wed:1;thu:0;fri:0;sat:1;sun:0";
            model.TimeToRun = "14:38";

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("dd:dddHHmm");

            var nextDate = DateTime.UtcNow;

            var foundDate = false;

            while (!foundDate)
            {
                if (nextDate.DayOfWeek == DayOfWeek.Saturday
                || nextDate.DayOfWeek == DayOfWeek.Wednesday
                || nextDate.DayOfWeek == DayOfWeek.Monday)
                {
                    foundDate = true;
                }
                else
                {
                    nextDate = nextDate.AddDays(1);
                }
            }

            var twleve = new TimeSpan(14, 38, 00);
            nextDate = nextDate.Date + twleve;

            Assert.AreEqual(nextDate.ToString("dd:dddHHmm"), nextRunTime);

        }

        [Test]
        public void NextRunIsNextMoTueAt0202()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 0;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "dayAndTime";
            model.DaysToRun = "mon:1;tue:1;wed:0;thu:0;fri:0;sat:0;sun:0";
            model.TimeToRun = "02:02";

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("dd:dddHHmm");

            var nextDate = DateTime.Now;

            var foundDate = false;
            var twleve = new TimeSpan(2, 2, 00);

            while (!foundDate)
            {
                if ((nextDate.DayOfWeek == DayOfWeek.Monday
                || nextDate.DayOfWeek == DayOfWeek.Tuesday))
                {
                    if (nextDate.TimeOfDay > twleve && DateTime.Now.DayOfWeek == nextDate.DayOfWeek)
                    {
                        nextDate = nextDate.AddDays(1);
                    }
                    else
                    {
                        foundDate = true;    
                    }  
                }
                else
                {
                    nextDate = nextDate.AddDays(1);
                }
            }

            
            nextDate = nextDate.Date + twleve;

            Assert.AreEqual(nextDate.ToString("dd:dddHHmm"), nextRunTime);

        }

        [Test]
        public void NextRunTimeHasPassedTodaySoNeedsToBeNextDay()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 0;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "dayAndTime";
            model.DaysToRun = "mon:1;tue:1;wed:1;thu:1;fri:1;sat:1;sun:1";
            var testingDate = DateTime.UtcNow.AddMinutes(-10);
            model.TimeToRun = testingDate.ToString("HH:mm");

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("dd:dddHHmm");

            var nextDate = DateTime.UtcNow;

            nextDate = nextDate.AddDays(1);

            var twleve = new TimeSpan(testingDate.Hour, testingDate.Minute, 00);
            nextDate = nextDate.Date + twleve;

            Assert.AreEqual(nextDate.ToString("dd:dddHHmm"), nextRunTime);

        }

        [Test]
        public void NextRunTimeHasntPassedTodaySoNeedsToBeNextDay()
        {
            var model = new ScheduleUrl();
            //model.Url = "test.co.uk";
            model.MinuteInterval = 0;
            model.LastRun = DateTime.UtcNow;
            model.RunningType = "dayAndTime";
            model.DaysToRun = "mon:1;tue:1;wed:1;thu:1;fri:1;sat:1;sun:1";
            var testingDate = DateTime.UtcNow.AddMinutes(10);
            model.TimeToRun = testingDate.ToString("HH:mm");

            var result = _timingService.GetNextRun(model);

            var nextRunTime = result.ToString("dd:dddHHmm");

            var nextDate = DateTime.UtcNow;

            //nextDate = nextDate.AddDays(1);

            var twleve = new TimeSpan(testingDate.Hour, testingDate.Minute, 00);
            nextDate = nextDate.Date + twleve;

            Assert.AreEqual(nextDate.ToString("dd:dddHHmm"), nextRunTime);

        }
    }
}
