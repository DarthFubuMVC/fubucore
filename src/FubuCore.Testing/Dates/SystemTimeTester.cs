using System;
using System.Collections.Generic;
using FubuCore.Dates;
using NUnit.Framework;

namespace FubuCore.Testing.Dates
{
    [TestFixture]
    public class SystemTimeTester
    {
        [Test]
        public void local_now()
        {
            var now = SystemTime.Default().LocalTime().Time;
            var secondNow = DateTime.Now;

            secondNow.Subtract(now).TotalSeconds.ShouldBeLessThan(1);
        }



        [Test]
        public void get_today()
        {
            SystemTime.Default().LocalTime().Date.Day.ShouldEqual(DateTime.Today);
        }

        [Test]
        public void current_time()
        {
            var now = SystemTime.Default().LocalTime().TimeOfDay;
            var secondNow = DateTime.Now.TimeOfDay;

            secondNow.Subtract(now).TotalSeconds.ShouldBeLessThan(1);
        }

        [Test]
        public void Setup()
        {
            var now = DateTime.Today.AddDays(1).AddHours(8);

            var clock = new Clock();
            var systemTime = new SystemTime(clock, new MachineTimeZoneContext());
            clock.LocalNow(now);

            systemTime.LocalTime().Time.ShouldEqual(now);
            systemTime.LocalTime().TimeOfDay.ShouldEqual(800.ToTime());
            systemTime.LocalTime().Date.Day.ShouldEqual(DateTime.Today.AddDays(1));
        }

        [Test]
        public void stub_then_back_to_live()
        {
            var now = DateTime.Today.AddDays(1).AddHours(8);

            var clock = new Clock();
            var systemTime = new SystemTime(clock, new MachineTimeZoneContext());
            clock.LocalNow(now);

            systemTime.LocalTime().Time.ShouldEqual(now);

            clock.Live();

            var firstNow = SystemTime.Default().LocalTime().Time;
            var secondNow = DateTime.Now;

            secondNow.Subtract(firstNow).TotalSeconds.ShouldBeLessThan(1);
        }

        [Test]
        public void time_zone_is_used_to_calculate_local_time()
        {
            TimeZoneInfo.GetSystemTimeZones().Each(zone =>
            {
                var time = new SystemTime(new Clock(), new SimpleTimeZoneContext(zone));
                var first = time.LocalTime().Time;
                var second = DateTime.UtcNow.ToLocalTime(zone);

                second.Subtract(first).TotalMilliseconds.ShouldBeLessThan(100);
            });
        }

        [Test]
        public void stub_by_using_at_local_time_by_time()
        {
            var systemTime = SystemTime.AtLocalTime("0700".ToTime());
            systemTime.LocalTime().ShouldEqual(LocalTime.AtMachineTime("0700"));
        }
    }
}