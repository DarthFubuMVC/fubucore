using System.Linq;
using FubuCore.Binding;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class PropertyBinderCacheTester
    {
        [Test]
        public void has_the_attribute_property_binder_by_default()
        {
            new PropertyBinderCache(new IPropertyBinder[0], null, null).Binders.Any(x => x is AttributePropertyBinder)
                .ShouldBeTrue();
        }
    }
}