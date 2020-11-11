using FubuCore.Util;
using NUnit.Framework;

namespace FubuCore.Testing.Util
{
    [TestFixture]
    public class StringPropertyReaderTester
    {
        [Test]
        public void read_simple_properties()
        {
            var text = @"
a=1
b=2
c=3
abcd=4
";

            var props = StringPropertyReader.ForText(text).ReadProperties();

            props["a"].ShouldEqual("1");
            props["b"].ShouldEqual("2");
            props["c"].ShouldEqual("3");
            props["abcd"].ShouldEqual("4");

        }

        [Test]
        public void use_the_trimmed_thing_to_work()
        {
            var text = @"
first=0
     .second=1
            .third=2
";

            var props = StringPropertyReader.ForText(text).ReadProperties();

            props["first"].ShouldEqual("0");
            props["first.second"].ShouldEqual("1");
            props["first.second.third"].ShouldEqual("2");
        }
    }
}