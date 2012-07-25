using FubuCore.Binding.InMemory;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ArrayConverterFamilyTester
    {
        public class HerpDerp
        {
            public string[] Strings { get; set; }
            public int[] Numbers { get; set; }
        }

        [Test]
        public void will_convert_comma_delimited_list_into_int_array()
        {
            BindingScenario<HerpDerp>.For(setup =>
            {
                setup.Data("Numbers=1, 2, 3"); 
            }).Model.Numbers.ShouldHaveTheSameElementsAs(1,2,3);
        }

        [Test]
        public void will_convert_comma_delimited_list_into_string_array()
        {
            BindingScenario<HerpDerp>.For(setup =>
            {
                setup.Data("Strings=herp, derp"); 
            }).Model.Strings.ShouldHaveTheSameElementsAs("herp","derp");
        }
    }
}