using System.Diagnostics;
using FubuCore.Binding;
using FubuCore.Descriptions;
using FubuCore.Testing.Binding;
using NUnit.Framework;

namespace FubuCore.Testing.Descriptions
{
    [TestFixture]
    public class DescriptionTextWriterSmokeTester
    {
        [Test]
        public void can_write_out_binding_registry_description()
        {
            var bindingRegistry = new BindingRegistry();
            bindingRegistry.Add(new FakeModelBinder());

            var description = Description.For(bindingRegistry);
            var writer = new DescriptionTextWriter(description);

            Debug.WriteLine(writer.ToString());
        }
    }
}