using System;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BindingAttributeTester
    {
        [Test]
        public void matches_negative_if_no_attribute()
        {
            var property = ReflectionHelper.GetProperty<FakeTarget>(x => x.Color3);
            new AttributePropertyBinder().Matches(property).ShouldBeFalse();
        }

        [Test]
        public void matches_positive_if_no_attribute()
        {
            var property = ReflectionHelper.GetProperty<FakeTarget>(x => x.Color1);
            new AttributePropertyBinder().Matches(property).ShouldBeTrue();
        }



        [Test]
        public void use_binding_attribute()
        {
            var target = BindingScenario<FakeTarget>.For(x =>
            {
                x.BindPropertyWith<AttributePropertyBinder>(o => o.Color1);
                x.BindPropertyWith<AttributePropertyBinder>(o => o.Color2);
            }).Model;

            target.Color1.ShouldEqual("red");
            target.Color2.ShouldEqual("green");
        }
    }


    public class FakeTarget
    {
        [FakeBinding("red")]
        public string Color1 { get; set; }

        [FakeBinding("green")]
        public string Color2 { get; set; }

        public string Color3 { get; set; }
    }

    public class FakeBindingAttribute : BindingAttribute
    {
        private readonly string _color;

        public FakeBindingAttribute(string color)
        {
            _color = color;
        }

        public override void Bind(PropertyInfo property, IBindingContext context)
        {
            property.SetValue(context.Object, _color, null);
        }
    }
}