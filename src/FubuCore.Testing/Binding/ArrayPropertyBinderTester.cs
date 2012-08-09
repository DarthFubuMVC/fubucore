using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Conversion;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ArrayPropertyBinderTester
    {
        [Test]
        public void matches_an_array_of_element_type_that_a_converter_library_does_not_understand()
        {
            var library = new ConverterLibrary();
            library.CanBeParsed(typeof(Target)).ShouldBeFalse();

            var property = GetType().GetProperty("MyTarget");

            new ArrayPropertyBinder(null).Matches(property).ShouldBeTrue();
        }

        [Test]
        public void definitely_does_not_match_a_non_array()
        {
            var property = GetType().GetProperty("Something");

            new ArrayPropertyBinder(null).Matches(property).ShouldBeFalse();
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
            public HybridGuy[] Guys { get; set; }
            public int[] Ages { get; set; }
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



        public string Something { get; set; }
        public int[] Numbers { get; set; }
        public Target[] MyTarget { get; set; }

        public class Target {}
    }
}