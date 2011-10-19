using System;
using FubuCore.Reflection.Expressions;
using FubuCore.Util;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuCore.Testing.Reflection.Expressions
{
    public class StubArguments : IArguments
    {
        private readonly Cache<string, object> _values = new Cache<string, object>();

        public T Get<T>(string propertyName)
        {
            return (T)_values[propertyName];
        }

        public bool Has(string propertyName)
        {
            return _values.Has(propertyName);
        }

        public void Set(string propertyName, object value)
        {
            _values[propertyName] = value;
        }
    }

    [TestFixture]
    public class ConstructorFunctionBuilderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var builder = new ConstructorFunctionBuilder<ConstructorTarget>();
            func = builder.CreateBuilder(typeof(ConstructorTarget).GetConstructors().First());
        }

        #endregion

        private Func<IArguments, ConstructorTarget> func;

        public class ConstructorTarget
        {
            private readonly int _age;
            private readonly DateTime _birthDay;
            private readonly string _name;

            public ConstructorTarget(string name, int age, DateTime birthDay)
            {
                _name = name;
                _age = age;
                _birthDay = birthDay;
            }

            public string Name { get { return _name; } }
            public int Age { get { return _age; } }
            public DateTime BirthDay { get { return _birthDay; } }


            public string Color { get; set; }
            public int Number { get; set; }
        }

        [Test]
        public void build_an_object()
        {
            var args = new StubArguments();
            args.Set("name", "Jeremy");
            args.Set("age", 35);

            // That's actually correct, you know, just in case you want to buy me
            // a birthday present
            args.Set("birthDay", new DateTime(1974, 1, 1));

            ConstructorTarget target = func(args);

            target.Name.ShouldEqual("Jeremy");
            target.Age.ShouldEqual(35);
            target.BirthDay.ShouldEqual(new DateTime(1974, 1, 1));
        }
    }
}