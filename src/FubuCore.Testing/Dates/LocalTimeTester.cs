using System;
using System.Diagnostics;
using FubuCore.Dates;
using NUnit.Framework;

namespace FubuCore.Testing.Dates
{
    [TestFixture]
    public class LocalTimeTester
    {
        [Test]
        public void dehydrate_and_hydrate()
        {
            var time = LocalTime.AtMachineTime("0800");
            var text = time.Hydrate();

            Debug.WriteLine(text);

            var time2 = new LocalTime(text);

            time2.UtcTime.ShouldEqual(time.UtcTime);

            time2.ShouldNotBeTheSameAs(time);
            time2.ShouldEqual(time);
        }

        [Test]
        public void hydrate_with_only_time()
        {
            new LocalTime("0800").ShouldEqual(LocalTime.AtMachineTime("0800"));
        }

        [Test]
        public void hydrate_with_date_and_time()
        {
            var expectedDay = new Date("26022012");
            new LocalTime("26022012:0800").ShouldEqual(LocalTime.AtDayAndTime(expectedDay, "0800".ToTime()));
        }

        [Test]
        public void create_local_time()
        {
            var time = new LocalTime(DateTime.Today.AddHours(8).ToUniversalTime(TimeZoneInfo.Local), TimeZoneInfo.Local);
            time.Time.ShouldEqual(DateTime.Today.AddHours(8));
            time.TimeOfDay.ShouldEqual("0800".ToTime());
            time.Date.ShouldEqual(DateTime.Today.ToDate());

            time.UtcTime.ShouldEqual(DateTime.Today.AddHours(8).ToUniversalTime(TimeZoneInfo.Local));
        }

        [Test]
        public void add()
        {
            var time = new LocalTime(DateTime.Today.AddHours(8), TimeZoneInfo.Local);
            var halfHourLater = time.Add("0800".ToTime());

            halfHourLater.Time.ShouldEqual(DateTime.Today.AddHours(16));
        }

        [Test]
        public void less_than()
        {
            var time1 = new LocalTime(DateTime.Today.AddHours(8), TimeZoneInfo.Local);
            var time2 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);
        
            (time1 < time2).ShouldBeTrue();
            (time2 < time1).ShouldBeFalse();
        }

        [Test]
        public void less_than_or_equal()
        {
            var time1 = new LocalTime(DateTime.Today.AddHours(8), TimeZoneInfo.Local);
            var time2 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);
            var time3 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);

            (time1 <= time2).ShouldBeTrue();
            (time2 <= time1).ShouldBeFalse();
            (time2 <= time3).ShouldBeTrue();
            (time3 <= time2).ShouldBeTrue();
        }

        [Test]
        public void greater_than_operator()
        {
            var time1 = new LocalTime(DateTime.Today.AddHours(8), TimeZoneInfo.Local);
            var time2 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);

            (time1 > time2).ShouldBeFalse();
            (time2 > time1).ShouldBeTrue();
        }

        [Test]
        public void greater_than_or_equal_operator()
        {
            var time1 = new LocalTime(DateTime.Today.AddHours(8), TimeZoneInfo.Local);
            var time2 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);
            var time3 = new LocalTime(DateTime.Today.AddHours(10), TimeZoneInfo.Local);

            (time1 >= time2).ShouldBeFalse();
            (time2 >= time1).ShouldBeTrue();
        
            (time2 >= time3).ShouldBeTrue();
            (time3 >= time2).ShouldBeTrue();
        }

        [Test]
        public void beginning_of_day()
        {
            var morningTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(8)); // 8 in the morning
            morningTime.BeginningOfDay().UtcTime.AddHours(8).ShouldEqual(morningTime.UtcTime);
        }

        [Test]
        public void GuessDayFromTimeOfDay()
        {
            var morningTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(8)); // 8 in the morning

            LocalTime.GuessDayFromTimeOfDay(morningTime, "0600".ToTime()).ShouldEqual(morningTime.Add(-2.Hours()));
            LocalTime.GuessDayFromTimeOfDay(morningTime, "0900".ToTime()).ShouldEqual(morningTime.Add(1.Hours()));
            LocalTime.GuessDayFromTimeOfDay(morningTime, "1000".ToTime()).ShouldEqual(morningTime.Add(2.Hours()));
            LocalTime.GuessDayFromTimeOfDay(morningTime, "1500".ToTime()).ShouldEqual(morningTime.Add(7.Hours()));
        }

        [Test]
        public void guess_day_should_find_tomorrow()
        {
            var currentTime = LocalTime.AtMachineTime("2300");

            LocalTime.GuessDayFromTimeOfDay(currentTime, 800.ToTime())
                .ShouldEqual(currentTime.Add(9.Hours()));
        }

        [Test]
        public void should_find_yesterday()
        {
            var currentTime = LocalTime.AtMachineTime("0300");

            LocalTime.GuessDayFromTimeOfDay(currentTime, 2100.ToTime())
                .ShouldEqual(currentTime.Add(-6.Hours()));
        }

        [Test]
        public void subtract()
        {
            var firstTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(8));
            var secondTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(12));

            secondTime.Subtract(firstTime).ShouldEqual(4.Hours());
        }


    }
}