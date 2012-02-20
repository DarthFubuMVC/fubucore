using System;
using System.Linq;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using FubuCore.Testing.Reflection.Expressions;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{

    [TestFixture]
    public class when_populating_a_property : InteractionContext<StandardModelBinder>
    {
        private PropertyInfo theProperty;
        private IPropertyBinder thePropertyBinder;

        protected override void beforeEach()
        {
            theProperty = ReflectionHelper.GetProperty<Case>(x => x.Identifier);

            MockFor<IBindingContext>().Stub(x => x.Logger).Return(MockFor<IBindingLogger>());

            thePropertyBinder = MockFor<IPropertyBinder>();
            MockFor<IPropertyBinderCache>().Stub(x => x.BinderFor(theProperty))
                .Return(thePropertyBinder);

            ClassUnderTest.PopulateProperty(typeof(Case), theProperty, MockFor<IBindingContext>());
        }

        [Test]
        public void should_log_the_property_binder_chosen()
        {
            MockFor<IBindingLogger>().AssertWasCalled(x => x.ChosePropertyBinder(theProperty, thePropertyBinder));
        }
    }


    [TestFixture]
    public class StandardModelBinderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            theScenario = null;
        }

        #endregion

        private BindingScenario<Turkey> theScenario;

        private void usingData(Action<BindingScenario<Turkey>.ScenarioDefinition> configuration)
        {
            theScenario = BindingScenario<Turkey>.For(configuration);
        }



        private Turkey theResultingObject { get { return theScenario.Model; } }

        private class Turkey
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public int? NullableInt { get; set; }
            public bool Alive { get; set; }
            public DateTime BirthDate { get; set; }
            public Guid Id { get; set; }
            public bool X_Requested_With { get; set; }
        }

        private class Duck
        {
            public int WingSpan { get; set; }
            public Turkey Turducken { get; set; }
        }

        [Test]
        public void Multiple_levels_of_bound_objects()
        {
            var scenario = BindingScenario<Duck>.For(x =>
            {
                x.Data("WingSpan", "7");
                x.Data("TurduckenName", "Bob");
            });

            scenario.Problems.Any().ShouldBeFalse();
            scenario.Model.WingSpan.ShouldEqual(7);
            scenario.Model.Turducken.Name.ShouldEqual("Bob");
        }


        [Test]
        public void
            Checkbox_handling__if_the_property_type_is_boolean_and_the_value_does_not_equal_the_name_and_isnt_a_recognizeable_boolean_a_problem_should_be_attached
            ()
        {
            usingData(x => x.Data(o => o.Alive, "BOGUS"));

            ConvertProblem problem = theScenario.Problems.Single();

            problem.Property.Name.ShouldEqual("Alive");
        }

        [Test]
        public void
            Checkbox_handling__if_the_property_type_is_boolean_and_the_value_equals_the_name_then_set_the_property_to_true
            ()
        {
            usingData(x => x.Data(o => o.Alive, "Alive"));

            theResultingObject.Alive.ShouldBeTrue();
        }

        [Test]
        public void create_and_populate_should_convert_between_types()
        {
            usingData(x =>
            {
                x.Data(@"
Age=12
Alive=True
BirthDate=01-JUN-2008
");
            });


            theResultingObject.Age.ShouldEqual(12);
            theResultingObject.Alive.ShouldBeTrue();
            theResultingObject.BirthDate.ShouldEqual(new DateTime(2008, 06, 01));
        }

        [Test, Ignore("Removed requirement for case-insensitivity. May add back later")]
        public void
            create_and_populate_should_create_new_object_and_set_all_property_values_present_in_dictionary_regardless_of_key_casing
            ()
        {
            Assert.Fail("Do.");
            //var dict = new Dictionary<string, object> { { "nAme", "Sally" }, { "AGE", 12 } };

            //var item = new DictionaryConverter().ConvertFrom<Turkey>(dict, out _problems);
            //item.Name.ShouldEqual("Sally");
            //item.Age.ShouldEqual(12);
        }

        [Test]
        public void create_and_populate_should_not_throw_exception_during_type_conversion_and_return_a_meaningful_error()
        {
            usingData(x =>
            {
                x.Data("Age", "abc");
            });

            theResultingObject.Age.ShouldEqual(default(int));

            var problem = theScenario.Problems.Single();

            problem.ExceptionText.ShouldContain("FormatException");
            problem.Item.ShouldBeTheSameAs(theResultingObject);
            problem.Property.Name.ShouldEqual("Age");
            problem.Value.RawValue.ShouldEqual("abc");
        }

        [Test]
        public void does_not_match_class_without_no_arg_ctor()
        {
            new StandardModelBinder(new BindingRegistry(), new TypeDescriptorCache()).Matches(typeof (ClassWithoutNoArgCtor)).ShouldBeFalse();
        }

        [Test]
        public void matches_class_with_no_arg_ctor()
        {
            new StandardModelBinder(new BindingRegistry(), new TypeDescriptorCache()).Matches(typeof(ClassWithNoArgCtor)).ShouldBeTrue();
        }

        [Test]
        public void no_errors_on_clean_transfer_of_valid_properties_to_object()
        {
            usingData(x => x.Data(@"
Name=Boris
Age=2
"));
            theScenario.Problems.Any().ShouldBeFalse();
        }

        [Test]
        public void populate_extra_values_in_dictionary_are_ignored()
        {
            usingData(x => x.Data("xyzzy", "foo"));

            theScenario.Problems.Count.ShouldEqual(0);

            theResultingObject.Name.ShouldBeNull();
            theResultingObject.Age.ShouldEqual(0);
        }

        [Test]
        public void populate_should_not_change_property_values_not_found_in_the_dictionary()
        {
            usingData(x =>
            {
                x.Data("Age", 9);
                x.Model.Name = "Smith";
            });

            theResultingObject.Name.ShouldEqual("Smith");
            theResultingObject.Age.ShouldEqual(9);
        }

        [Test]
        public void populate_should_set_all_property_values_present_in_dictionary()
        {
            usingData(x =>
            {
                x.Data("Name", "Boris");
                x.Data("Age", "2");
            });

            theResultingObject.Name.ShouldEqual("Boris");
            theResultingObject.Age.ShouldEqual(2);
        }

        [Test, Ignore("Removed requirement for case-insensitivity. May add back later")]
        public void populate_should_set_all_property_values_present_in_dictionary_regardless_of_key_casing()
        {
            Assert.Fail("Do.");
            //var item = new Turkey();

            //var dict = new Dictionary<string, object> { { "nAme", "Smith" }, { "AGE", 9 } };

            //new DictionaryConverter().Populate(dict, item, out _problems);
            //item.Name.ShouldEqual("Smith");
            //item.Age.ShouldEqual(9);
        }


        [Test]
        public void Read_a_boolean_type_that_is_false()
        {
            usingData(x => x.Data("Alive", ""));


            theResultingObject.Alive.ShouldBeFalse();
        }

        [Test]
        public void Read_a_boolean_type_that_is_true()
        {
            usingData(x => x.Data("Alive", "true"));
            theResultingObject.Alive.ShouldBeTrue();
        }

        [Test]
        public void Read_a_Nullable_value_type()
        {
            usingData(x => x.Data("NullableInt", "8"));
            
            
            theResultingObject.NullableInt.ShouldEqual(8);
        }

        [Test]
        public void Read_a_Nullable_value_type_empty_string_as_null()
        {
            usingData(x => x.Data("NullableInt", string.Empty));
            theResultingObject.NullableInt.ShouldBeNull();

            theScenario.Problems.Count.ShouldEqual(0);
        }

        [Test]
        public void should_convert_from_string_to_guid()
        {
            Guid guid = Guid.NewGuid();
            usingData(x => x.Data("Id", guid.ToString()));
            
            theResultingObject.Id.ShouldEqual(guid);
        }

        [Test]
        public void should_use_alternate_underscore_naming_if_primary_fails()
        {
            usingData(x => x.Data("X-Requested-With", "True"));

            theResultingObject.X_Requested_With.ShouldBeTrue();
        }
    }

    public class ClassWithoutNoArgCtor
    {
        public ClassWithoutNoArgCtor(int something)
        {
        }
    }

    public class ClassWithNoArgCtor
    {
    }
}