using System;
using FubuCore.Dates;
using NUnit.Framework;
using FubuTestingSupport;

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
            var time2 = new LocalTime(text);

            time2.UtcTime.ShouldEqual(time.UtcTime);

            time2.ShouldNotBeTheSameAs(time);
            time2.ShouldEqual(time);
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
        public void determine_utc_time_without_base_time()
        {
            var morningTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(8)); // 8 in the morning

            LocalTime.GuessTime(morningTime, "0600".ToTime()).ShouldEqual(morningTime.Add(-2.Hours()));
            LocalTime.GuessTime(morningTime, "0900".ToTime()).ShouldEqual(morningTime.Add(1.Hours()));
            LocalTime.GuessTime(morningTime, "1000".ToTime()).ShouldEqual(morningTime.Add(2.Hours()));
            LocalTime.GuessTime(morningTime, "1500".ToTime()).ShouldEqual(morningTime.Add(7.Hours()));
        }

        [Test]
        public void subtract()
        {
            var firstTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(8));
            var secondTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(12));

            secondTime.Subtract(firstTime).ShouldEqual(4.Hours());
        }

        [Test]
        public void guess_utc_time_with_base_time()
        {
            var eveningTime = LocalTime.AtMachineTime(DateTime.Today.AddHours(22));
            var floor = eveningTime.BeginningOfDay().AtTime(700.ToTime()).UtcTime;

            
            LocalTime.GuessTime(eveningTime, "2300".ToTime(), floor).ShouldEqual(eveningTime.Add(1.Hours()));

            // rolls over to the next day
            LocalTime.GuessTime(eveningTime, "0100".ToTime(), floor).ShouldEqual(eveningTime.Add(3.Hours()));
        }
    }
}