using System;
using FubuCore.Reflection.Expressions;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Expressions
{
    [TestFixture]
    public class ConstructorBuilderTester
    {
        [Test]
        public void build_the_func()
        {
            var ctor = ConstructorBuilder.CreateSingleStringArgumentConstructor(typeof (Component));
            ctor.Compile().As<Func<string, Component>>()("something")
                .Text.ShouldEqual("something");
        }
    }

    public class Component
    {
        private readonly string _text;

        public Component(string text)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }
    }
}