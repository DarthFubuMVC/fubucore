using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Conversion;
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
            propertyBinder = new CollectionPropertyBinder(null);
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
            shouldNotMatch(x => x.StringArray);
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


        [Test]
        public void apply_collection_that_can_be_converted_as_individual_values()
        {
            var guys = BindingScenario<HybridHolder>.Build(x =>
            {
                x.Data(@"
Guys[0]Name=Jeremy
Guys[0]Age=38
Guys[1]Name=Max
Guys[1]Age=8
");
            }).Guys;

            guys.ShouldHaveTheSameElementsAs(new HybridGuy("Jeremy", 38), new HybridGuy("Max", 8));
        }

        [Test]
        public void apply_collection_binding_when_it_is_one_value_should_delegate_to_the_conversion()
        {
            var guys = BindingScenario<HybridHolder>.Build(x =>
            {
                x.Data(@"
Guys=Jeremy:38,Max:8
");
            }).Guys;

            guys.ShouldHaveTheSameElementsAs(new HybridGuy("Jeremy", 38), new HybridGuy("Max", 8));
        }

        [Test]
        public void do_a_primitive_list()
        {
            BindingScenario<HybridHolder>.Build(x =>
            {
                x.Data("Ages", "38,8,32");
            }).Ages.ShouldHaveTheSameElementsAs(38, 8, 32);
        }

        public class HybridHolder
        {
            public IList<HybridGuy> Guys { get; set; }
            public IList<int> Ages { get; set; }
        }

        public class HybridGuy
        {
            public HybridGuy()
            {
            }

            public HybridGuy(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public HybridGuy(string text)
            {
                var parts = text.Split(':');
                Name = parts.First();
                Age = int.Parse(parts.Last());
            }

            public string Name { get; set; }
            public int Age { get; set; }

            public bool Equals(HybridGuy other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Name, Name) && other.Age == Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(HybridGuy)) return false;
                return Equals((HybridGuy)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }

            public override string ToString()
            {
                return string.Format("Name: {0}, Age: {1}", Name, Age);
            }
        }
    }


}