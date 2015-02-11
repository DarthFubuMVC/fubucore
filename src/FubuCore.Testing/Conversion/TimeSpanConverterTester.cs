using System;
using FubuCore.Conversion;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Conversion
{
    [TestFixture]
    public class TimeSpanConverterTester
    {
        [Test]
        public void happily_converts_timespans_in_4_digit_format()
        {
            TimeSpanConverter.GetTimeSpan("1230").ShouldEqual(new TimeSpan(12, 30, 0));
        }

        [Test]
        public void happily_converts_timespans_in_5_digit_format()
        {
            TimeSpanConverter.GetTimeSpan("12:30").ShouldEqual(new TimeSpan(12, 30, 0));
        }

        [Test]
        public void converts_timespans_for_seconds()
        {
            TimeSpanConverter.GetTimeSpan("3.5s").ShouldEqual(TimeSpan.FromSeconds(3.5));
            TimeSpanConverter.GetTimeSpan("5 s").ShouldEqual(TimeSpan.FromSeconds(5));
            TimeSpanConverter.GetTimeSpan("1 second").ShouldEqual(TimeSpan.FromSeconds(1));
            TimeSpanConverter.GetTimeSpan("12 seconds").ShouldEqual(TimeSpan.FromSeconds(12));
        }

        [Test]
        public void converts_timespans_for_minutes()
        {
            TimeSpanConverter.GetTimeSpan("10m").ShouldEqual(TimeSpan.FromMinutes(10));
            TimeSpanConverter.GetTimeSpan("2.1 m").ShouldEqual(TimeSpan.FromMinutes(2.1));
            TimeSpanConverter.GetTimeSpan("1 minute").ShouldEqual(TimeSpan.FromMinutes(1));
            TimeSpanConverter.GetTimeSpan("5 minutes").ShouldEqual(TimeSpan.FromMinutes(5));
        }

        [Test]
        public void converts_timespans_for_hours()
        {
            TimeSpanConverter.GetTimeSpan("24h").ShouldEqual(TimeSpan.FromHours(24));
            TimeSpanConverter.GetTimeSpan("4 h").ShouldEqual(TimeSpan.FromHours(4));
            TimeSpanConverter.GetTimeSpan("1 hour").ShouldEqual(TimeSpan.FromHours(1));
            TimeSpanConverter.GetTimeSpan("12.5 hours").ShouldEqual(TimeSpan.FromHours(12.5));
        }

        [Test]
        public void converts_timespans_for_days()
        {
            TimeSpanConverter.GetTimeSpan("3d").ShouldEqual(TimeSpan.FromDays(3));
            TimeSpanConverter.GetTimeSpan("2 d").ShouldEqual(TimeSpan.FromDays(2));
            TimeSpanConverter.GetTimeSpan("1 day").ShouldEqual(TimeSpan.FromDays(1));
            TimeSpanConverter.GetTimeSpan("7 days").ShouldEqual(TimeSpan.FromDays(7));
        }

        [Test]
        public void can_convert_from_standard_format()
        {
            TimeSpanConverter.GetTimeSpan("00:00:01").ShouldEqual(new TimeSpan(0, 0, 1));
            TimeSpanConverter.GetTimeSpan("00:10:00").ShouldEqual(new TimeSpan(0, 10, 0));
            TimeSpanConverter.GetTimeSpan("01:30:00").ShouldEqual(new TimeSpan(1, 30, 0));
            TimeSpanConverter.GetTimeSpan("1.01:30:00").ShouldEqual(new TimeSpan(1, 1, 30, 0));
            TimeSpanConverter.GetTimeSpan("-00:10:00").ShouldEqual(new TimeSpan(0, -10, 0));
            TimeSpanConverter.GetTimeSpan("12:34:56.789").ShouldEqual(new TimeSpan(0, 12, 34, 56, 789));
        }
    }
}