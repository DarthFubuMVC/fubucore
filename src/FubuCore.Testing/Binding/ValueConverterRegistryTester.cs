using System.Reflection;
using System.Web;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Conversion;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ValueConverterRegistryTester
    {
        public class Target
        {
            public int Integer { get; set; }
            public bool Boolean { get; set; }
            public int? NullInt { get; set; }
        }

        public class TargetHolder
        {
            public Target Target { get; set; }
        }

        [Test]
        public void return_a_null_converter()
        {
            var property = ReflectionHelper.GetProperty<TargetHolder>(x => x.Target);
            new BindingRegistry().As<IValueConverterRegistry>().FindConverter(property).ShouldBeNull();
        }

        [Test]
        public void should_convert_nonnull_values_for_nullable_types()
        {
            BindingScenario<Target>.Build(x =>
            {
                x.Data(o => o.NullInt, 99);
            }).NullInt.ShouldEqual(99);
        }

        [Test]
        public void should_convert_null_values_for_nullable_types()
        {
            BindingScenario<Target>.Build(x =>
            {
                
            }).NullInt.ShouldBeNull();
        }




    }
}