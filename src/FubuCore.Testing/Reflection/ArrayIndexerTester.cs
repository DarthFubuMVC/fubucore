using System;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection
{
    [TestFixture]
    public class ArrayIndexerTester
    {
        public class Target
        {
            public string Name { get; set; }
        }

        [Test]
        public void DeclaringType_of_a_array_is_the_array_type()
        {
            var accessor = ReflectionHelper.GetAccessor<Target[]>(x => x[1]);
            accessor.ShouldBeOfType<ArrayIndexer>();
            accessor.OwnerType.ShouldEqual(typeof (Target[]));
            accessor.DeclaringType.ShouldEqual(typeof (Target[]));
            accessor.FieldName.ShouldEqual("[1]");
            accessor.InnerProperty.ShouldBeNull();
            accessor.PropertyNames.ShouldContain("[1]");
            accessor.PropertyType.ShouldEqual(typeof (Target));
        }

        [Test]
        public void GetValueFromArray()
        {
            var accessor = ReflectionHelper.GetAccessor<Target[]>(x => x[1]);

            var target = new[] {new Target(), new Target() };

            accessor.GetValue(target).ShouldEqual(target[1]);
            accessor.GetValue(target).ShouldNotEqual(target[0]);
        }

        [Test]
        public void SetValueOnArray()
        {
            var accessor = ReflectionHelper.GetAccessor<Target[]>(x => x[1]);

            var original = new Target();
            var replacement = new Target();
            var target = new[] {new Target(), original };

            accessor.SetValue(target, replacement);
            target[1].ShouldNotEqual(original);
            target[1].ShouldEqual(replacement);
        }

        [Test]
        public void ExpressionCreation()
        {
            var accessor = ReflectionHelper.GetAccessor<Target[]>(x => x[1]);

            var target = new[] {new Target(), new Target() };
            accessor.ToExpression<Target[]>().Compile()(target).ShouldEqual(target[1]);
        }

        [Test]
        public void ExpressionCreationWithValueType()
        {
            var accessor = ReflectionHelper.GetAccessor<DateTime[]>(x => x[1]);

            var target = new[] { new DateTime(2015, 01, 01), new DateTime(2015, 01, 02) };
            accessor.ToExpression<DateTime[]>().Compile()(target).ShouldEqual(target[1]);
        }

        [Test]
        public void SetValueOnArrayWithValueType()
        {
            var accessor = ReflectionHelper.GetAccessor<DateTime[]>(x => x[1]);

            var original = new DateTime(2015, 01, 01);
            var replacement = new DateTime(2015, 01, 02);
            var target = new[] { new DateTime(2015, 01, 03), original };

            accessor.SetValue(target, replacement);
            target[1].ShouldNotEqual(original);
            target[1].ShouldEqual(replacement);
        }
   }
}