using System.Collections.Generic;
using FubuCore.Binding.InMemory;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class CollectionBinding_Acceptance_Tests
    {
        [Test]
        public void can_bind_collection()
        {
            var scenario = BindingScenario<AddressViewModel>.For(x =>
            {
                x.Data(@"
Localities[0]ZipCode=72712
Localities[0]CountyName=Benton
Localities[1]ZipCode=64755
Localities[1]CountyName=Jasper
Localities[2]ZipCode=78750
Localities[2]CountyName=Travis
");
            });

            scenario.Model.Localities.ShouldHaveTheSameElementsAs(
                new LocalityViewModel("72712", "Benton"), 
                new LocalityViewModel("64755", "Jasper"), 
                new LocalityViewModel("78750", "Travis"));
        }

        [Test]
        public void can_bind_list()
        {
            var scenario = BindingScenario<LocalityList>.For(x =>
            {
                x.Data(@"
Localities[0]ZipCode=72712
Localities[0]CountyName=Benton
Localities[1]ZipCode=64755
Localities[1]CountyName=Jasper
Localities[2]ZipCode=78750
Localities[2]CountyName=Travis
");
            });

            scenario.Model.Localities.ShouldHaveTheSameElementsAs(
                new LocalityViewModel("72712", "Benton"),
                new LocalityViewModel("64755", "Jasper"),
                new LocalityViewModel("78750", "Travis"));
        }


        [Test]
        public void can_bind_enumerable()
        {
            var scenario = BindingScenario<LocalityEnumerable>.For(x =>
            {
                x.Data(@"
Localities[0]ZipCode=72712
Localities[0]CountyName=Benton
Localities[1]ZipCode=64755
Localities[1]CountyName=Jasper
Localities[2]ZipCode=78750
Localities[2]CountyName=Travis
");
            });

            scenario.Model.Localities.ShouldHaveTheSameElementsAs(
                new LocalityViewModel("72712", "Benton"),
                new LocalityViewModel("64755", "Jasper"),
                new LocalityViewModel("78750", "Travis"));
        }

        [Test]
        public void can_bind_array()
        {
            var scenario = BindingScenario<LocalityArray>.For(x =>
            {
                x.Data(@"
Localities[0]ZipCode=72712
Localities[0]CountyName=Benton
Localities[1]ZipCode=64755
Localities[1]CountyName=Jasper
Localities[2]ZipCode=78750
Localities[2]CountyName=Travis
");
            });

            scenario.Model.Localities.ShouldHaveTheSameElementsAs(
                new LocalityViewModel("72712", "Benton"),
                new LocalityViewModel("64755", "Jasper"),
                new LocalityViewModel("78750", "Travis"));
        }

        [Test]
        public void can_bind_collection_on_a_leaf_property()
        {
            var scenario = BindingScenario<HasAddress>.For(x =>
            {
                x.Data(@"
Name=Jeremy
AddressDescription=the house down the road
AddressLocalities[0]ZipCode=72712
AddressLocalities[0]CountyName=Benton
AddressLocalities[1]ZipCode=64755
AddressLocalities[1]CountyName=Jasper
AddressLocalities[2]ZipCode=78750
AddressLocalities[2]CountyName=Travis
");
            });

            scenario.Model.Address.Localities.ShouldHaveTheSameElementsAs(
                new LocalityViewModel("72712", "Benton"),
                new LocalityViewModel("64755", "Jasper"),
                new LocalityViewModel("78750", "Travis"));

            scenario.Model.Address.Description.ShouldEqual("the house down the road");
            scenario.Model.Name.ShouldEqual("Jeremy");
        }
    }

    public class LocalityList
    {
        public IList<LocalityViewModel> Localities { get; set; }
    }

    public class LocalityEnumerable
    {
        public IEnumerable<LocalityViewModel> Localities { get; set; }
    }

    public class LocalityArray
    {
        public LocalityViewModel[] Localities { get; set; }
    }

    public class HasAddress
    {
        public AddressViewModel Address { get; set; }
        public string Name { get; set; }
    }
}