using FubuCore.Binding.InMemory;
using FubuCore.Conversion;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Binding.InMemory
{
    [TestFixture]
    public class InMemoryServiceLocatorTester
    {
        [Test]
        public void add_and_retrieve_service()
        {
            var service = new InMemoryServiceLocator();

            service.Add<int>(5);

            service.GetInstance<int>().ShouldEqual(5);
        }

        [Test]
        public void object_converter_is_registered_automatically()
        {
            new InMemoryServiceLocator().GetInstance<IObjectConverter>()
                .ShouldBeOfType<ObjectConverter>();
        }
    }
}