using FubuCore.Binding;
using FubuCore.Conversion;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ArrayPropertyBinderTester
    {
        [Test]
        public void matches_an_array_of_element_type_that_a_converter_library_does_not_understand()
        {
            var library = new ConverterLibrary();
            library.CanBeParsed(typeof(Target)).ShouldBeFalse();

            var property = GetType().GetProperty("MyTarget");

            new ArrayPropertyBinder(library).Matches(property).ShouldBeTrue();
        }

        [Test]
        public void does_not_match_an_array_that_can_be_parsed()
        {
            var property = GetType().GetProperty("Numbers");

            new ArrayPropertyBinder(new ConverterLibrary()).Matches(property).ShouldBeFalse();
        }

        [Test]
        public void definitely_does_not_match_a_non_array()
        {
            var property = GetType().GetProperty("Something");

            new ArrayPropertyBinder(new ConverterLibrary()).Matches(property).ShouldBeFalse();
        }

        public string Something { get; set; }
        public int[] Numbers { get; set; }
        public Target[] MyTarget { get; set; }

        public class Target {}
    }
}