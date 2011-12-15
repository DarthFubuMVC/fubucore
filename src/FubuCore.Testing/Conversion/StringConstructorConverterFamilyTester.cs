using FubuCore.Conversion;
using FubuCore.Testing.Reflection.Expressions;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Conversion
{
    [TestFixture]
    public class StringConstructorConverterFamilyTester
    {
        [Test]
        public void matches_positive()
        {
            var family = new StringConstructorConverterFamily();
            family.Matches(typeof(Component), null).ShouldBeTrue();
        }

        public class NotTheRightThingHere{}

        [Test]
        public void matches_negative()
        {
            new StringConstructorConverterFamily()
                .Matches(typeof (NotTheRightThingHere), null)
                .ShouldBeFalse();
        }

        [Test]
        public void create_converter()
        {
            var func = new StringConstructorConverterFamily().CreateConverter(typeof (Component), null);
            func.Convert("something").ShouldBeOfType<Component>()
                .Text.ShouldEqual("something");
        }
    }
}