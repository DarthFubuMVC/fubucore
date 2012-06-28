using System;
using FubuCore.Dates;
using NUnit.Framework;
using FubuTestingSupport;

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
    }
}