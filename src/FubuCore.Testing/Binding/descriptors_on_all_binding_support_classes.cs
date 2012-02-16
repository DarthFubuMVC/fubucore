using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuCore.Descriptions;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class descriptors_on_all_binding_support_classes
    {
        [Test]
        public void must_be_a_description_on_all_conversion_families()
        {
            var types = typeof(IConverterFamily).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IConverterFamily>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Debug.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }
    }
}