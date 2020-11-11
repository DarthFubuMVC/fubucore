using System.Linq;
using FubuCore.Binding;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class PropertyBinderCacheTester
    {
        [Test]
        public void has_the_attribute_property_binder_by_default()
        {
            new BindingRegistry().AllPropertyBinders().OfType<AttributePropertyBinder>().Count().ShouldEqual(1);
        }
    }
}