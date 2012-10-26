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

        [Test]
        public void can_write_an_object_with_properties_and_children()
        {
            // Honestly just doing an eyeball check on this
            var parent = new DescribedParent();
            parent.WriteDescriptionToConsole();
            
        }
    }

    public class DescribedParent : DescribesItself
    {
        public void Describe(Description description)
        {
            description.Title = "The described parent";
            description.Properties["Color"] = "Orange";
            description.Properties["Direction"] = "North";
            description.Properties["Name"] = "Max";

            description.AddChild("Child1 Something", new DescribedChild());
            description.AddChild("Child2 else", new DescribedChild());
            description.AddChild("Child3", new DescribedChild());
        }
    }

    [Description("I'm a described child")]
    public class DescribedChild
    {
        
    }
}