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
        public void now()
        {
            var now = new SystemTime().Now();
            var secondNow = DateTime.Now;

            secondNow.Subtract(now).TotalSeconds.ShouldBeLessThan(1);
        }

        [Test]
        public void get_today()
        {
            new SystemTime().Today().Day.ShouldEqual(DateTime.Today);
        }

        [Test]
        public void current_time()
        {
            var now = new SystemTime().CurrentTime();
            var secondNow = DateTime.Now.TimeOfDay;

            secondNow.Subtract(now).TotalSeconds.ShouldBeLessThan(1);
        }

        [Test]
        public void stub()
        {
            var now = DateTime.Today.AddDays(1).AddHours(8);

            var systemTime = new SystemTime();
            systemTime.Now(now);

            systemTime.Now().ShouldEqual(now);
            systemTime.CurrentTime().ShouldEqual(800.ToTime());
            systemTime.Today().Day.ShouldEqual(DateTime.Today.AddDays(1));
        }

        [Test]
        public void stub_then_back_to_live()
        {
            var now = DateTime.Today.AddDays(1).AddHours(8);

            var systemTime = new SystemTime();
            systemTime.Now(now);

            systemTime.Now().ShouldEqual(now);

            systemTime.Live();

            var firstNow = new SystemTime().Now();
            var secondNow = DateTime.Now;

            secondNow.Subtract(firstNow).TotalSeconds.ShouldBeLessThan(1);
        }
    }
}