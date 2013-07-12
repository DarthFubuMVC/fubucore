using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection
{
    [TestFixture]
    public class MethodValueGetterTester
    {
        [Test]
        public void should_get_hashcode()
        {
            var accessor = ReflectionHelper.GetAccessor<TestSubject>(x => x.Value());
            accessor.GetHashCode().ShouldNotBeNull();
        }
    }

    public class TestSubject
    {
        public object Value()
        {
            return new object();
        }
    }
}
