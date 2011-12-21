using System;
using FubuCore.Conversion;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Conversion
{
    [TestFixture]
    public class TimeSpanConverterTester
    {
        [Test]
        public void happily_converts_timespans_in_4_digit_format()
        {
            TimeSpanConverter.GetTimeSpan("1230").ShouldEqual(new TimeSpan(12, 30, 0));
        }

        [Test]
        public void happily_converts_timespans_in_5_digit_format()
        {
            TimeSpanConverter.GetTimeSpan("12:30").ShouldEqual(new TimeSpan(12, 30, 0));
        }
    }
}