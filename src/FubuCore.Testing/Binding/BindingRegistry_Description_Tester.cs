using System;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuCore.Descriptions;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BindingRegistry_Description_Tester
    {
        private Description theDescription;

        [SetUp]
        public void SetUp()
        {
            var registry = new BindingRegistry();
            registry.Add(new FakeModelBinder());
            theDescription = Description.For(registry);
        }

        [Test]
        public void description_has_a_bullet_list_for_binders()
        {
            var list = theDescription.BulletLists.First(x => x.Name == "ModelBinders");
            list.Label.ShouldEqual("Model Binders (IModelBinder)");
            list.IsOrderDependent.ShouldBeTrue();
        }

        [Test]
        public void the_model_binder_bullet_list_has_a_description_for_each_model_binder_in_order()
        {
            var list = theDescription.BulletLists.First(x => x.Name == "ModelBinders");
            list.Children.Select(x => x.TargetType)
                .ShouldHaveTheSameElementsAs(typeof(FakeModelBinder), typeof(StandardModelBinder));
        }

    }

    [TestFixture]
    public class when_building_the_description_for_the_standard_model_binder
    {
        private Description theDescription  ;
        private BulletList thePropertyBinderList;
        private BindingRegistry theRegistry;

        [SetUp]
        public void SetUp()
        {
            theRegistry = new BindingRegistry();
            theDescription = Description.For(theRegistry).BulletLists.Single().Children.Single();

            thePropertyBinderList = theDescription.BulletLists.Single();
        }

        [Test]
        public void got_names_for_all_the_property_binders()
        {
            thePropertyBinderList.Name.ShouldEqual("Property Binders");
        }

        [Test]
        public void the_property_binder_list_is_order_dependent()
        {
            thePropertyBinderList.IsOrderDependent.ShouldBeTrue();
        }

        [Test]
        public void has_a_property_binder_description_in_the_list_for_each_property_binder()
        {
            thePropertyBinderList.Children.Select(x => x.TargetType)
                .ShouldHaveTheSameElementsAs(theRegistry.AllPropertyBinders().Select(x => x.GetType()));
        }
    }

    [TestFixture]
    public class when_building_the_description_for_the_conversion_property_binder
    {
        private BindingRegistry theRegistry;
        private Description theDescription;
        private BulletList theConversionList;

        [SetUp]
        public void SetUp()
        {
            theRegistry = new BindingRegistry();
            var binder = new ConversionPropertyBinder(theRegistry);
            theDescription = Description.For(binder);

            theConversionList = theDescription.BulletLists.Single();
        }

        [Test]
        public void the_name_and_label_of_the_conversion_list()
        {
            theConversionList.Name.ShouldEqual("ConversionFamilies");
            theConversionList.Label.ShouldEqual("Conversion Families");
        }

        [Test]
        public void the_conversion_list_must_be_marked_as_order_dependent()
        {
            theConversionList.IsOrderDependent.ShouldBeTrue();
        }

        [Test]
        public void the_list_has_a_description_for_each_conversion_family()
        {
            theConversionList.Children.Select(x => x.TargetType)
                .ShouldHaveTheSameElementsAs(theRegistry.AllConverterFamilies().Select(x => x.GetType()));
        }


    }

    [TestFixture]
    public class when_describing_the_BasicConverterFamily
    {
        private ConverterLibrary theLibrary;
        private Description theDescription;
        private BulletList theFamilyList;

        [SetUp]
        public void SetUp()
        {
            theLibrary = new ConverterLibrary();
            var basicConverterFamily = new BasicConverterFamily(theLibrary);

            theDescription = Description.For(basicConverterFamily);

            // Just lifts the Family list right off of ConverterLibrary
            theFamilyList = theDescription.BulletLists.Single();
        }

        [Test]
        public void family_list_must_be_ordered()
        {
            theFamilyList.IsOrderDependent.ShouldBeTrue();
        }

        [Test]
        public void the_family_list_labels()
        {
            theFamilyList.Name.ShouldEqual(typeof(IObjectConverterFamily).Name);
            theFamilyList.Label.ShouldEqual("All registered " + typeof(IObjectConverterFamily).Name + "'s");
        }

        [Test]
        public void has_a_description_for_each_IObjectConverterFamily()
        {
            theFamilyList.Children.Select(x => x.TargetType)
                .ShouldHaveTheSameElementsAs(theLibrary.AllConverterFamilies.Select(x => x.GetType()));
        }
    }

    [TestFixture]
    public class when_describing_the_converter_library
    {
        private ConverterLibrary theLibrary;
        private Description theDescription;
        private BulletList theFamilyList;

        [SetUp]
        public void SetUp()
        {
            theLibrary = new ConverterLibrary();

            theDescription = Description.For(theLibrary);
            theFamilyList = theDescription.BulletLists.Single();
        }

        [Test]
        public void family_list_must_be_ordered()
        {
            theFamilyList.IsOrderDependent.ShouldBeTrue();
        }

        [Test]
        public void the_family_list_labels()
        {
            theFamilyList.Name.ShouldEqual(typeof (IObjectConverterFamily).Name);
            theFamilyList.Label.ShouldEqual("All registered " + typeof (IObjectConverterFamily).Name + "'s");
        }

        [Test]
        public void has_a_description_for_each_IObjectConverterFamily()
        {
            theFamilyList.Children.Select(x => x.TargetType)
                .ShouldHaveTheSameElementsAs(theLibrary.AllConverterFamilies.Select(x => x.GetType()));
        }
    }

    public class FakeModelBinder : IModelBinder
    {
        public bool Matches(Type type)
        {
            throw new NotImplementedException();
        }

        public void BindProperties(Type type, object instance, IBindingContext context)
        {
            throw new NotImplementedException();
        }

        public object Bind(Type type, IBindingContext context)
        {
            throw new NotImplementedException();
        }
    }
}