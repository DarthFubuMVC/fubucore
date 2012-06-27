using System;
using FubuCore.Dates;
using NUnit.Framework;
using FubuTestingSupport;
using FubuCore;

namespace FubuCore.Testing.Dates
{
    [TestFixture]
    public class SystemTimeTester
    {
        [Test]
        public void local_now()
        {
            var now = SystemTime.Default().LocalNow();
            var secondNow = DateTime.Now;

            secondNow.Subtract(now).TotalSeconds.ShouldBeLessThan(1);
        }



        [Test]
        public void get_today()
        {
            SystemTime.Default().LocalDay().Day.ShouldEqual(DateTime.Today);
        }

        [Test]
        public void current_time()
        {
            var now = SystemTime.Default().LocalTime();
            var secondNow = DateTime.Now.TimeOfDay;

            secondNow.Subtract(now).TotalSeconds.ShouldBeLessThan(1);
        }

        [Test]
        public void stub()
        {
            var now = DateTime.Today.AddDays(1).AddHours(8);

            var clock = new Clock();
            var systemTime = new SystemTime(clock, new MachineTimeZoneContext());
            clock.LocalNow(now);

            systemTime.LocalNow().ShouldEqual(now);
            systemTime.LocalTime().ShouldEqual(800.ToTime());
            systemTime.LocalDay().Day.ShouldEqual(DateTime.Today.AddDays(1));
        }

        [Test]
        public void stub_then_back_to_live()
        {
            var now = DateTime.Today.AddDays(1).AddHours(8);

            var clock = new Clock();
            var systemTime = new SystemTime(clock, new MachineTimeZoneContext());
            clock.LocalNow(now);

            systemTime.LocalNow().ShouldEqual(now);

            clock.Live();

            var firstNow = SystemTime.Default().LocalNow();
            var secondNow = DateTime.Now;

            secondNow.Subtract(firstNow).TotalSeconds.ShouldBeLessThan(1);
        }
    }
}