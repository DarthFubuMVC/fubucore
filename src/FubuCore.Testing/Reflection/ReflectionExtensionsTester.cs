using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection
{
    [TestFixture]
    public class ReflectionExtensionsTester
    {
        public class PropertyHolder{
            public int Age { get; set; }}
        public interface ICallback{void Callback();}

        private Mock<ICallback> _callback;
        private Expression<Func<PropertyHolder, object>> _expression;
        private Mock<ICallback> _uncalledCallback;

        [SetUp]
        public void SetUp()
        {
            _expression = ph => ph.Age;
            _callback = new Mock<ICallback>();
            _uncalledCallback = new Mock<ICallback>();
        }

        [Test]
        public void get_name_returns_expression_property_name()
        {
            _expression.GetName().ShouldEqual("Age");
        }

        [Test]
        public void ifPropertyTypeIs_invokes_method()
        {
            Accessor accessor = _expression.ToAccessor();
            accessor.IfPropertyTypeIs<int>(_callback.Object.Callback);
            _callback.Verify(c=>c.Callback());
            accessor.IfPropertyTypeIs<PropertyHolder>(_uncalledCallback.Object.Callback);
            _uncalledCallback.VerifyNotCalled(c=>c.Callback());
        }

        [Test]
        public void isInteger_returns_if_accessor_property_type_is_int()
        {
            Accessor accessor = _expression.ToAccessor();
            accessor.IsInteger().ShouldBeTrue();
        }
    }
}