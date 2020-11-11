using System;
using FubuCore.Dates;
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
            thePeriod = new Period(DateTime.Now.ToLocal());
        }

        [Test]
        public void mark_completed()
        {
            var completedTime = DateTime.Now.ToLocal();
            thePeriod.MarkCompleted(completedTime);
            thePeriod.To.ShouldEqual(completedTime);
        }

        [Test]
        public void is_active_at_with_open_to()
        {
            thePeriod.To.ShouldBeNull();
            thePeriod.IsActiveAt(thePeriod.From).ShouldBeTrue();

            thePeriod.IsActiveAt(thePeriod.From.Add(3.Days())).ShouldBeTrue();


            thePeriod.IsActiveAt(thePeriod.From.Add(-1.Days())).ShouldBeFalse();
        }

        [Test]
        public void is_active_when_the_boundary_is_closed()
        {
            thePeriod.MarkCompleted(thePeriod.From.Add(2.Days()));

            thePeriod.IsActiveAt(thePeriod.From).ShouldBeTrue();
            thePeriod.IsActiveAt(thePeriod.From.Add(1.Minutes())).ShouldBeTrue();
            thePeriod.IsActiveAt(thePeriod.From.Add(-1.Minutes())).ShouldBeFalse();

            // NOT inclusive
            thePeriod.IsActiveAt(thePeriod.To).ShouldBeFalse();
            thePeriod.IsActiveAt(thePeriod.To.Add(1.Minutes())).ShouldBeFalse();

        }

        [Test]
        public void find_date_time_within()
        {
            var today = DateTime.Today.ToLocal();
            var from = today.Add(7.Hours());
            var to = today.Add(31.Hours());

            var period = new Period(from, to);

            period.FindDateTime("0700").ShouldEqual(today.Add(7.Hours()));
            period.FindDateTime("0800").ShouldEqual(today.Add(8.Hours()));
            period.FindDateTime("2300").ShouldEqual(today.Add(23.Hours()));
            period.FindDateTime("0500").ShouldEqual(today.Add(29.Hours())); // early morning the next day
            period.FindDateTime("0300").ShouldEqual(today.Add(27.Hours())); // early morning the next day

        }
    }
}