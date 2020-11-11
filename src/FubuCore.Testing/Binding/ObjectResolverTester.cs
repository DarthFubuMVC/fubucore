using System;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{

    [TestFixture]
    public class object_resolver_throws_exception_on_bind_model_with_no_matching_model_binder : InteractionContext<ObjectResolver>
    {
        [Test]
        public void throw_fubu_exception_if_there_is_no_suitable_binder()
        {
            MockFor<IModelBinderCache>().Setup(x => x.BinderFor(GetType())).Returns((IModelBinder)null);


            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.BindModel(typeof(ClassWithNoCtor), MockFor<IBindingContext>().Object);
            }).ErrorCode.ShouldEqual(2200);



        }

        public class ClassWithNoCtor
        {
            private ClassWithNoCtor()
            {
            }
        }
    }

    [TestFixture]
    public class when_binding_model_throws_exception
    {
        private Mock<IModelBinder> matchingBinder;
        private Type _type = typeof(BinderTarget);

        [Test]
        public void should_throw_fubu_exception_2201()
        {
            matchingBinder = new Mock<IModelBinder>();
            matchingBinder.Setup(x => x.Matches(_type)).Returns(true);
            matchingBinder
                .Setup(x => x.Bind(Arg<Type>.Is.Equal(_type), Arg<IBindingContext>.Is.Anything))
                .Throws(new Exception("fake message"));


            var exception = Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                BindingScenario<BinderTarget>.Build(x =>
                {
                    x.Registry.Add(matchingBinder.Object);
                });
            });

            exception.ShouldNotBeNull().Message.ShouldEqual(
                "FubuCore Error 2201:  \nFatal error while binding model of type {0}.  See inner exception"
                .ToFormat(_type.AssemblyQualifiedName));
        }
    }


    [TestFixture, Ignore("Need an option in FubuCore's BindingScenario so that it's using BindModel(type, foo) instead of BindModel(type, instance, foo)")]
    public class fetching_an_object_should_choose_the_first_model_binder_applicable
    {
        private Mock<IModelBinder> binder1;
        private Mock<IModelBinder> binder2;
        private IModelBinder matchingBinder;
        private BinderTarget expectedResult;

        [Test]
        public void should_resolve_the_requested_model_with_the_first_binder_that_matches()
        {
            binder1 = new Mock<IModelBinder>();
            binder2 = new Mock<IModelBinder>();

            binder1.Setup(x => x.Matches(typeof(BinderTarget))).Returns(false);
            binder2.Setup(x => x.Matches(typeof(BinderTarget))).Returns(false);

            expectedResult = new BinderTarget();

            matchingBinder = new StubBinder(expectedResult);

            BindingScenario<BinderTarget>.Build(x =>
            {
                x.Registry.Add(binder1.Object);
                x.Registry.Add(binder2.Object);
                x.Registry.Add(matchingBinder);
            }).ShouldBeTheSameAs(expectedResult);
        }
    }

    public class StubBinder : IModelBinder
    {
        private readonly BinderTarget _target;

        public StubBinder(BinderTarget target)
        {
            _target = target;
        }

        public bool Matches(Type type)
        {
            return type == typeof (BinderTarget);
        }

        public void BindProperties(Type type, object instance, IBindingContext context)
        {
            throw new NotImplementedException();
        }

        public object Bind(Type type, IBindingContext context)
        {
            return _target;
        }
    }

    [TestFixture]
    public class fetching_an_object_that_had_conversion_problems
    {
        [Test]
        public void the_conversion_problems_should_be_recorded()
        {
            BindingScenario<BinderTarget>.For(x =>
            {
                x.Data("Age", "abc");
                x.Data("Name", "Jeremy");
            }).Problems.Count.ShouldEqual(1);

        }
    }


    public class BinderTargetBase
    {
    }

    public class BinderTarget : BinderTargetBase
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsTrue { get; set; }
    }


}