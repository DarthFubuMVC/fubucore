using System.Reflection;
using FubuCore.Reflection;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection
{
    [TestFixture]
    public class MethodValueGetterTester
    {
        private readonly MethodInfo TheMethodInfo = ReflectionHelper.GetMethod<TestSubject>(x => x.Value());
        private readonly Accessor TheAccessor = ReflectionHelper.GetAccessor<TestSubject>(x => x.Value());
        private readonly Accessor TheArgAccessor = ReflectionHelper.GetAccessor<TestSubject>(x => x.AnotherMethod("Test"));
        private readonly MethodInfo TheArgMethodInfo = ReflectionHelper.GetMethod<TestSubject>(x => x.AnotherMethod("Test"));

        [Test]
        public void hashcode_should_not_eq_zero()
        {
            TheAccessor.GetHashCode().ShouldNotEqual(0);
        }

        [Test]
        public void should_return_methodinfo_hash()
        {
            var expectedHash = TheMethodInfo.GetHashCode();
            TheAccessor.GetHashCode().ShouldEqual(expectedHash);
        }

        [Test]
        public void with_arguments_should_get_correct_hashcode()
        {
            var actual = TheArgAccessor.GetHashCode();
            var expectedHash = (TheArgMethodInfo.GetHashCode() * 397) ^ ("Test".GetHashCode());
            actual.ShouldEqual(expectedHash);
        }
    }

    public class TestSubject
    {
        public object Value()
        {
            return new object();
        }

        public object AnotherMethod(string arg1)
        {
            return arg1;
        }
    }
}
