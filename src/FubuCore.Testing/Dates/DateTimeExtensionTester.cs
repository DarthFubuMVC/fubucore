using System;
using FubuCore.Dates;
using NUnit.Framework;

namespace FubuCore.Testing.Dates
{
    [TestFixture]
    public class DateTimeExtensionTester
    {
        [Test]
        public void to_date()
        {
            // not everything is hard
            DateTime.Now.ToDate().Day.ShouldEqual(DateTime.Today);
        }

        [Test]
        public void first_day_of_month()
        {
            var date = new DateTime(2012, 6, 7);
            date.FirstDayOfMonth().ShouldEqual(new DateTime(2012, 6, 1).ToDate());
        }

        [Test]
        public void last_day_of_month()
        {
            var date = new DateTime(2012, 6, 7);
            date.LastDayOfMonth().ShouldEqual(new DateTime(2012, 6, 30).ToDate());
        }

        [Test]
        public void end_of_month_calcs_properly_mid_month_input()
        {
            var realLastDay = new DateTime(2012, 02, 29);
            var today = new DateTime(2012, 02, 03);
            today.LastDayOfMonth().ShouldEqual(realLastDay.ToDate());
        }

        [Test]
        public void eom_as_input_calcs_begin_and_end_properly()
        {
            var realLastDay = new DateTime(2012, 02, 29);
            var realFirstDay = new DateTime(2012, 02, 01);
            var today = new DateTime(2012, 02, 29);
            today.LastDayOfMonth().ShouldEqual(realLastDay.ToDate());
            today.FirstDayOfMonth().ShouldEqual(realFirstDay.ToDate());
        }

        [Test]
        public void first_day_of_month_for_date_is_same()
        {
            var today = new DateTime(2012, 02, 01);
            today.FirstDayOfMonth().ShouldEqual(today.ToDate());
        }

        [Test]
        public void middle_of_month_input_has_proper_first_day_of_month()
        {
            var realFirstDay = new DateTime(2012, 02, 01);
            var today = new DateTime(2012, 02, 03);
            today.FirstDayOfMonth().ShouldEqual(realFirstDay.ToDate());
        }
    }
}