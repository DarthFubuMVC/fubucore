using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class CollectionPropertyBinderTester : PropertyBinderTester
    {
        [SetUp]
        public void Setup()
        {
            context = new InMemoryBindingContext();
            var objectResolver = ObjectResolver.Basic();
            context.RegisterService<IObjectResolver>(objectResolver);
            context.RegisterService<ISmartRequest>(new InMemorySmartRequest());
            context.RegisterService<IRequestData>(new InMemoryRequestData());
            
            propertyBinder = new CollectionPropertyBinder(new DefaultCollectionTypeProvider());
        }

        [Test]
        public void matches_collection_properties()
        {
            shouldMatch(x => x.Localities);
        }

        [Test]
        public void doesnt_match_any_other_properties()
        {
            shouldNotMatch(x => x.Address);
            shouldNotMatch(x => x.Address.Address1);
            shouldNotMatch(x => x.Address.Order);
            shouldNotMatch(x => x.Address.IsActive);
            shouldNotMatch(x => x.Address.DateEntered);
            shouldNotMatch(x => x.Address.Color);
            shouldNotMatch(x => x.Address.Guid);
        }

        [Test]
        public void set_a_property_correctly_against_a_binding_context()
        {
            var model = new AddressViewModel();
            context.WithData("Localities[0]ZipCode", "84115");
            context.StartObject(model);

            var property = ReflectionHelper.GetProperty<AddressViewModel>(x => x.Localities);
            propertyBinder.Bind(property, context);

            model.Localities[0].ZipCode.ShouldEqual("84115");
        }

        [Test]
        public void existing_collection_is_not_discarded()
        {
            var model = new AddressViewModel
            {
                Localities = new List<LocalityViewModel>
                {
                    new LocalityViewModel {ZipCode = "previously_set_zipcode"}
                }
            };

            context.WithData("Localities[0]ZipCode", "84115");
            context.StartObject(model);

            var property = ReflectionHelper.GetProperty<AddressViewModel>(x => x.Localities);
            propertyBinder.Bind(property, context);

            model.Localities[0].ZipCode.ShouldEqual("previously_set_zipcode");
            model.Localities[1].ZipCode.ShouldEqual("84115");
        }
    }
}