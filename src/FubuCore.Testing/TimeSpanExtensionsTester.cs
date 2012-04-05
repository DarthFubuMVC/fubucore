using System;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing
{
    [TestFixture]
    public class TimeSpanExtensionsTester
    {
        [Test]
        public void to_time_from_int()
        {
            700.ToTime().ShouldEqual(new TimeSpan(7, 0, 0));
            1700.ToTime().ShouldEqual(new TimeSpan(17, 0, 0));
            1850.ToTime().ShouldEqual(new TimeSpan(18, 50, 0));
        }

        [Test]
        public void to_time_from_string()
        {
            "0700".ToTime().ShouldEqual(new TimeSpan(7, 0, 0));
            "1700".ToTime().ShouldEqual(new TimeSpan(17, 0, 0));
            "1850".ToTime().ShouldEqual(new TimeSpan(18, 50, 0));
        }

        [Test]
        public void Minutes()
        {
            5.Minutes().ShouldEqual(new TimeSpan(0, 5, 0));
        }

        [Test]
        public void hours()
        {
            6.Hours().ShouldEqual(new TimeSpan(6, 0, 0));
        }

        [Test]
        public void days()
        {
            2.Days().ShouldEqual(new TimeSpan(2, 0, 0, 0));
        }

        [Test]
        public void seconds()
        {
            8.Seconds().ShouldEqual(new TimeSpan(0, 0, 8));
        }
    }
}