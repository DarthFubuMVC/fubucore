using System;
using FubuCore.Dates;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Dates
{
    [TestFixture]
    public class PeriodTester
    {
        private Period thePeriod;

        [SetUp]
        public void SetUp()
        {
            thePeriod = new Period(DateTime.Now);
        }

        [Test]
        public void mark_completed()
        {
            var completedTime = DateTime.Now;
            thePeriod.MarkCompleted(completedTime);
            thePeriod.To.Value.ShouldEqual(completedTime);
        }

        [Test]
        public void is_active_at_with_open_to()
        {
            thePeriod.To.HasValue.ShouldBeFalse();
            thePeriod.IsActiveAt(thePeriod.From).ShouldBeTrue();

            thePeriod.IsActiveAt(thePeriod.From.AddDays(3)).ShouldBeTrue();


            thePeriod.IsActiveAt(thePeriod.From.AddDays(-1)).ShouldBeFalse();
        }

        [Test]
        public void is_active_when_the_boundary_is_closed()
        {
            thePeriod.MarkCompleted(thePeriod.From.AddDays(2));

            thePeriod.IsActiveAt(thePeriod.From).ShouldBeTrue();
            thePeriod.IsActiveAt(thePeriod.From.AddMinutes(1)).ShouldBeTrue();
            thePeriod.IsActiveAt(thePeriod.From.AddMinutes(-1)).ShouldBeFalse();

            // NOT inclusive
            thePeriod.IsActiveAt(thePeriod.To.Value).ShouldBeFalse();
            thePeriod.IsActiveAt(thePeriod.To.Value.AddMinutes(1)).ShouldBeFalse();

        }

        [Test]
        public void find_date_time_within()
        {
            var today = DateTime.Today;
            var from = today.AddHours(7);
            var to = today.AddHours(31);

            var period = new Period(from, to);

            period.FindDateTime("0700").ShouldEqual(today.AddHours(7));
            period.FindDateTime("0800").ShouldEqual(today.AddHours(8));
            period.FindDateTime("2300").ShouldEqual(today.AddHours(23));
            period.FindDateTime("0500").ShouldEqual(today.AddHours(29)); // early morning the next day
            period.FindDateTime("0300").ShouldEqual(today.AddHours(27)); // early morning the next day

        }
    }
}