using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection
{
    [TestFixture]
    public class SingleMethodTester
    {
        [Test]
        public void should_be_serializable()
        {
            typeof(SingleMethod).IsSerializable.ShouldBeTrue();
        }
    }
}