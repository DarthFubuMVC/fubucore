using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Conversion;
using FubuCore.Reflection;
using FubuCore.Util;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BindingContextTester
    {
        private InMemoryRequestData request;
        private IServiceLocator locator;
        private BindingContext context;

        [SetUp]
        public void SetUp()
        {
            request = new InMemoryRequestData();
            locator = MockRepository.GenerateMock<IServiceLocator>();

            var smartRequest = MockRepository.GenerateMock<ISmartRequest>();
            locator.Stub(x => x.GetInstance<ISmartRequest>()).Return(smartRequest);

            context = new BindingContext(request, locator, new NulloBindingLogger());
        }

        [Test]
        public void get_regular_property()
        {
            request["Address1"] = "2035 Ozark";
            var property = ReflectionHelper.GetProperty<Address>(x => x.Address1);

            bool wasCalled = false;
            context.ForProperty(property, x =>
            {
                x.RawValueFromRequest.RawValue.ShouldEqual(request["Address1"]);

                wasCalled = true;
            });

            wasCalled.ShouldBeTrue();
        }

        [Test]
        public void get_property_that_falls_through_to_the_second_naming_strategy()
        {
            request["User-Agent"] = "hank";
            var property = ReflectionHelper.GetProperty<State>(x => x.User_Agent);

            bool wasCalled = false;

            context.ForProperty(property, x =>
            {
                x.RawValueFromRequest.RawValue.ShouldEqual("hank");
                x.RawValueFromRequest.RawKey.ShouldEqual("User-Agent");
                wasCalled = true;
            });

            wasCalled.ShouldBeTrue();
        }

        public class State
        {
            public string User_Agent { get; set; }
        }
    }


    /// <summary>
    /// This is an integration test
    /// </summary>
    [TestFixture]
    public class when_binding_a_child_property_with_valid_data
    {
        private HolderClass holder;
        private BindingScenario<HolderClass> theScenario;

        [SetUp]
        public void SetUp()
        {
            theScenario = BindingScenario<HolderClass>.For(x =>
            {
                x.Data(@"
HeldClassName=Jeremy
HeldClassAge=36
");
            });

            holder = theScenario.Model;
        }

        [Test]
        public void the_child_property_is_filled()
        {
            holder.HeldClass.ShouldNotBeNull();
        }

        [Test]
        public void the_properties_of_the_child_object_are_filled_in_from_the_request_data()
        {
            holder.HeldClass.Name.ShouldEqual("Jeremy");
            holder.HeldClass.Age.ShouldEqual(36);
        }

        [Test]
        public void should_be_no_problems_recorded()
        {
            theScenario.Problems.Any().ShouldBeFalse();
        }
    }


    [TestFixture]
    public class when_binding_a_child_object_that_has_some_invalid_data
    {
        private BindingScenario<HolderClass> theScenario;

        [SetUp]
        public void SetUp()
        {
            theScenario = BindingScenario<HolderClass>.For(x =>
            {
                x.Data("Number", "NOT A NUMBER");
                x.Data("HeldClassName", "Jeremy");
                x.Data("HeldClassAge", "NOT A NUMBER JEREMY");
                x.Data("Collection[0]Name", "Jaime");
                x.Data("Collection[0]Age", "28");
                x.Data("Collection[1]Name", "Peter");
                x.Data("Collection[1]Age", "NOT A NUMBER PETER");
                x.Data("Collection[2]Name", "Charles");
                x.Data("Collection[2]Age", "30");
                x.Data("Collection[3]Name", "Rose");
                x.Data("Collection[3]Age", "NOT A NUMBER ROSE");
            });
        }

        [Test]
        public void the_child_property_is_filled()
        {
            theScenario.Model.HeldClass.ShouldNotBeNull();
        }

        [Test]
        public void the_properties_of_the_child_object_are_filled_in_from_the_request_data()
        {
            theScenario.Model.HeldClass.Name.ShouldEqual("Jeremy");
            theScenario.Model.Collection.ShouldNotBeNull().ShouldHaveCount(4);
            theScenario.Model.Collection[0].Name.ShouldEqual("Jaime");
            theScenario.Model.Collection[0].Age.ShouldEqual(28);
            theScenario.Model.Collection[1].Name.ShouldEqual("Peter");
            theScenario.Model.Collection[2].Name.ShouldEqual("Charles");
            theScenario.Model.Collection[2].Age.ShouldEqual(30);
            theScenario.Model.Collection[3].Name.ShouldEqual("Rose");
        }

        [Test]
        public void should_record_all_the_problems()
        {
            var problems = theScenario.Problems.OrderBy(x => x.Accessor.PropertyNames.Length).ThenBy(x => x.Accessor.Name).ToList();
            problems.ShouldHaveCount(4);

            checkProblem(problems[0], typeof(HolderClass), x => x.Number, "NOT A NUMBER");
            checkProblem(problems[1], typeof(ClassThatIsHeld), x => x.HeldClass.Age, "NOT A NUMBER JEREMY");
            checkProblem(problems[2], typeof(ClassThatIsHeld), x => x.Collection[1].Age, "NOT A NUMBER PETER");
            checkProblem(problems[3], typeof(ClassThatIsHeld), x => x.Collection[3].Age, "NOT A NUMBER ROSE");
        }

        private static void checkProblem(ConvertProblem problem, Type itemType, Expression<Func<HolderClass, object>> exp, string error)
        {
            problem.Property.ShouldEqual(exp.ToAccessor().InnerProperty);
            problem.Item.ShouldBeOfType(itemType);
            problem.Accessor.Name.ShouldEqual(exp.ToAccessor().Name);
            problem.ExceptionText.ShouldContain(error);
        }
    }

    [TestFixture]
    public class value_of_scenarios
    {
        private InMemoryRequestData theRawRequest;
        private BindingContext ClassUnderTest;

        [SetUp]
        public void beforeEach()
        {
            theRawRequest = new InMemoryRequestData();

            var services = new InMemoryServiceLocator();
            services.Add<IObjectConverter>(new ObjectConverter());

            ClassUnderTest = new BindingContext(theRawRequest, services, new NulloBindingLogger());
        }

        [Test]
        public void value_as_t_by_name()
        {
            var theValue = Guid.NewGuid();
            var theKey = "some key";
            theRawRequest[theKey] = theValue;

            ClassUnderTest.As<IBindingContext>().Data.ValueAs<Guid>(theKey).ShouldEqual(theValue);
        }

        [Test]
        public void value_as_t_by_name_with_a_naming_strategy()
        {
            var theValue = Guid.NewGuid();
            var theKey = "some key";
            theRawRequest["[" + theKey + "]"] = theValue;

            ClassUnderTest.As<IBindingContext>().Data.ValueAs<Guid>(theKey).ShouldEqual(theValue);
        }


        [Test]
        public void value_as_t_by_name_with_continuation()
        {
            var action = MockRepository.GenerateMock<Action<Guid>>();
            var theKey = "some key";
            var theValue = Guid.NewGuid();

            theRawRequest[theKey] = theValue;

            ClassUnderTest.As<IBindingContext>().Data.ValueAs(theKey, action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke(theValue));
        }

        [Test]
        public void value_as_by_name_with_continuation()
        {
            var action = MockRepository.GenerateMock<Action<object>>();
            var theKey = "some key";
            var theValue = Guid.NewGuid();

            theRawRequest[theKey] = theValue;

            ClassUnderTest.As<IBindingContext>().Data.ValueAs(typeof(Guid), theKey, action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke(theValue));
        }

        [Test]
        public void value_by_name_with_continuation()
        {
            var theKey = "some key";
            var theValue = Guid.NewGuid();

            theRawRequest[theKey] = theValue;

            ClassUnderTest.As<IBindingContext>().Data.ValueAs(typeof (Guid), theKey).ShouldEqual(theValue);
        }

        [Test]
        public void value_as_t_from_property_info()
        {
            var property = ReflectionHelper.GetProperty<ClassThatIsHeld>(x => x.Name);

            var theValue = Guid.NewGuid();
            theRawRequest["Name"] = theValue;


            ClassUnderTest.ForProperty(property, context =>
            {
                context.ValueAs<Guid>().ShouldEqual(theValue);
            });
        }

        [Test]
        public void value_as_t_from_property_info_with_naming_strategy()
        {
            var property = ReflectionHelper.GetProperty<ClassThatIsHeld>(x => x.Name);

            var theValue = Guid.NewGuid();
            theRawRequest["[Name]"] = theValue;


            ClassUnderTest.ForProperty(property, context =>
            {
                context.ValueAs<Guid>().ShouldEqual(theValue);
            });
        }

        [Test]
        public void value_as_t_from_property_by_continuation()
        {
            var property = ReflectionHelper.GetProperty<ClassThatIsHeld>(x => x.Name);

            var theValue = Guid.NewGuid().ToString();
            theRawRequest["[Name]"] = theValue;
            theRawRequest["[Name]"] = theValue;

            var action = MockRepository.GenerateMock<Action<string>>();

            ClassUnderTest.ForProperty(property, context =>
            {
                context.ValueAs<string>(action).ShouldBeTrue();
            });

            action.AssertWasCalled(x => x.Invoke(theValue));
        }
    }

    [TestFixture]
    public class nested_object_and_logger
    {
        private IBindingLogger _logger;
        private BindingContext _context;
        private InMemoryRequestData _request;
        private PropertyInfo _fooProperty;

        [SetUp]
        public void Setup()
        {
            var services = new InMemoryServiceLocator();
            var resolver = new ObjectResolver(services, new BindingRegistry(), new NulloBindingLogger());
            _request = new InMemoryRequestData();
            _logger = MockRepository.GenerateMock<IBindingLogger>();
            _context = new BindingContext(_request, services, _logger);

            services.Add<IObjectResolver>(resolver);
            _request[ReflectionHelper.GetAccessor<ComplexClass>(x => x.Nested.Foo).Name] = "Bar";
            _fooProperty = ReflectionHelper.GetProperty<ComplexClass>(x => x.Nested.Foo);
            setup();
        }

        protected virtual void setup()
        {
        }

        [TestFixture]
        public class binding_object_with_type : nested_object_and_logger
        {
            protected override void setup()
            {
                _context.BindObject(typeof (ComplexClass), o => { });
            }

            [Test]
            public void logs_the_properties_using_the_context_logger()
            {
                _logger.AssertWasCalled(x => x.Chose(Arg<PropertyInfo>.Is.Equal(_fooProperty), Arg<IPropertyBinder>.Is.Anything));
            }
        }

        [TestFixture]
        public class binding_object_with_request_data : nested_object_and_logger
        {
            protected override void setup()
            {
                _context.BindObject(_request, typeof (ComplexClass), o => { });
            }

            [Test]
            public void logs_the_properties_using_the_context_logger()
            {
                _logger.AssertWasCalled(x => x.Chose(Arg<PropertyInfo>.Is.Equal(_fooProperty), Arg<IPropertyBinder>.Is.Anything));
            }
        }
    }


    public class ComplexClass
    {
        public NestedClass Nested { get; set; }
    }

    public class NestedClass
    {
        public string Foo { get; set; }
    }

    public class StubSmartRequest : ISmartRequest
    {
        private readonly Cache<string, object> _values = new Cache<string, object>();

        public object this[string key]
        {
            get
            {
                return _values[key];
            }
            set
            {
                _values[key] = value;
            }
        }

        public object Value(Type type, string key)
        {
            throw new NotImplementedException();
        }

        public bool Value(Type type, string key, Action<object> continuation)
        {
            throw new NotImplementedException();
        }

        public T Value<T>(string key)
        {
            return (T)_values[key];
        }

        public bool Value<T>(string key, Action<T> callback)
        {
            if (!_values.Has(key))
            {
                return false;
            }

            callback((T)_values[key]);

            return true;
        }
    }


    public class ClassThatIsHeld
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Active { get; set; }
    }

    public class SpecialClassThatIsHeld : ClassThatIsHeld
    {
        public string Color { get; set; }
    }

    public class HeldClassThatGetsRejected : ClassThatIsHeld
    {

    }

    public class HolderClass
    {
        private ClassThatIsHeld _heldClass;
        public ClassThatIsHeld HeldClass
        {
            get { return _heldClass; }
            set
            {
                if (value is HeldClassThatGetsRejected)
                {
                    throw new InvalidCastException("the exception message");
                }
                _heldClass = value;
            }
        }

        public List<ClassThatIsHeld> Collection { get; set; }

        public int Number { get; set; }
    }



}