using System;
using FubuCore.Dates;
using NUnit.Framework;

namespace FubuCore.Testing.Dates
{
    [TestFixture]
    public class DateTester
    {
        [Test]
        public void convert_by_constructor()
        {
            var date = new Date("22022012");
            date.Day.ShouldEqual(new DateTime(2012, 2, 22));
        }

        [Test]
        public void equals_method()
        {
            var date1 = new Date("22022012");
            var date2 = new Date("22022012");
            var date3 = new Date("22022013");

            date1.ShouldEqual(date2);
            date2.ShouldEqual(date1);

            date3.ShouldNotEqual(date1);
        }

        [Test]
        public void to_string_uses_the_ugly_ddMMyyyy_format()
        {
            var date = new Date(2, 22, 2012);
            date.ToString().ShouldEqual("22022012");
        }

        [Test]
        public void ctor_by_date_loses_the_time()
        {
            var date = new Date(DateTime.Now);
            date.Day.ShouldEqual(DateTime.Today);
        }
    }
}