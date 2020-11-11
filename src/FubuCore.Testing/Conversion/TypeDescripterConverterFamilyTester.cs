using System;
using FubuCore.Conversion;
using NUnit.Framework;

namespace FubuCore.Testing.Conversion
{
    [TestFixture]
    public class TypeDescripterConverterFamilyTester
    {
        [Test]
        public void matches_positive()
        {
            var family = new TypeDescripterConverterFamily();
            family.Matches(typeof(string), null).ShouldBeTrue();
            family.Matches(typeof(int), null).ShouldBeTrue();
            family.Matches(typeof(DateTime), null).ShouldBeTrue();
            family.Matches(typeof(bool), null).ShouldBeTrue();
        }

        [Test]
        public void matches_negative()
        {
            var family = new TypeDescripterConverterFamily();
            family.Matches(GetType(), null).ShouldBeFalse();
        }

        [Test]
        public void create_a_working_converter()
        {
            var family = new TypeDescripterConverterFamily();

            var boolConverter = family.CreateConverter(typeof (bool), null);
            var intConverter = family.CreateConverter(typeof (int), null);

            boolConverter.Convert("true").ShouldEqual(true);
            boolConverter.Convert("false").ShouldEqual(false);

            intConverter.Convert("123").ShouldEqual(123);
            intConverter.Convert("456").ShouldEqual(456);

        }
    }
}