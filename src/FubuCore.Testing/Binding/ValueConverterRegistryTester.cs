using System.Reflection;
using System.Web;
using FubuCore.Binding;
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
            new ValueConverterRegistry(new IConverterFamily[0]).FindConverter(property).ShouldBeNull();
        }

        [Test]
        public void should_convert_nonnull_values_for_nullable_types()
        {
            PropertyInfo nullIntProp = ReflectionHelper.GetProperty<Target>(x => x.NullInt);
            var reg = new ValueConverterRegistry(new IConverterFamily[0]);
            var value = new InMemoryBindingContext().WithPropertyValue("99");
            value.ForProperty(nullIntProp, c =>
            {
                reg.FindConverter(nullIntProp).Convert(c).ShouldEqual(99);
            });

            
        }

        [Test]
        public void should_convert_null_values_for_nullable_types()
        {
            PropertyInfo nullIntProp = ReflectionHelper.GetProperty<Target>(x => x.NullInt);
            var reg = new ValueConverterRegistry(new IConverterFamily[0]);

            var value = new InMemoryBindingContext().WithPropertyValue(null);
            value.ForProperty(nullIntProp, c =>
            {
                reg.FindConverter(nullIntProp).Convert(c).ShouldEqual(null);
            });

            
        }




    }
}