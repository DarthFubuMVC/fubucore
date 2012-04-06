using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class CollectionPropertyBinderTester : PropertyBinderTester
    {
        [SetUp]
        public void Setup()
        {
            propertyBinder = new CollectionPropertyBinder();
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
        public void existing_collection_is_not_discarded()
        {
            var originalList = new List<LocalityViewModel> { new LocalityViewModel { ZipCode = "previously_set_zipcode" } };

            var model = BindingScenario<AddressViewModel>.Build(x =>
            {
                x.Model = new AddressViewModel(){
                    Localities = originalList
                };

                x.Data("Localities[0]ZipCode", "84115");
            });

            model.Localities.Select(x => x.ZipCode).ShouldHaveTheSameElementsAs("previously_set_zipcode", "84115");
        }

    }
}