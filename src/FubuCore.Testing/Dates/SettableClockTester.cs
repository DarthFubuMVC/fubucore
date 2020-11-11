using System;
using FubuCore.Dates;
using NUnit.Framework;

namespace FubuCore.Testing.Dates
{
    [TestFixture]
    public class SettableClockTester
    {
        [Test]
        public void set_the_clock()
        {
            var clock = new SettableClock();
            var localNow = DateTime.Today.AddHours(8);

            clock.LocalNow(localNow, TimeZoneInfo.Local);

            clock.LocalTime().Time.ShouldEqual(localNow);

            clock.UtcNow().ShouldEqual(localNow.ToUniversalTime(TimeZoneInfo.Local));
        }

        [Test]
        public void set_the_clock_with_a_local_time()
        {
            var local = LocalTime.AtMachineTime("0800");

            var clock = new SettableClock();
            clock.LocalNow(local);

            clock.LocalTime().ShouldNotBeTheSameAs(local).ShouldEqual(local);

            clock.UtcNow().ShouldEqual(local.UtcTime);
        }
    }
}