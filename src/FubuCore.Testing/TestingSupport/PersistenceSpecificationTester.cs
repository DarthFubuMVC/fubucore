using System.Collections.Generic;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuCore.Testing.TestingSupport
{
    [TestFixture]
    public class PersistenceSpecificationTester
    {
        private PersistenceCheckTarget original;
        private PersistenceCheckTarget persisted;
        private PersistenceSpecification<PersistenceCheckTarget> specification;

        [SetUp]
        public void SetUp()
        {
            original = new PersistenceCheckTarget();
            persisted = new PersistenceCheckTarget();

            specification = new PersistenceSpecification<PersistenceCheckTarget>(o => persisted);
            specification.Original = original;
        }

        [Test]
        public void build_accessor_for_an_enumerable()
        {
            PersistenceSpecification<PersistenceCheckTarget>.BuildCheck(x => x.Names)
                .ShouldBeOfType<EnumerablePersistenceCheck<string>>()
                .Accessor.Name.ShouldEqual("Names");
        }

        [Test]
        public void build_accessor_for_a_non_enumerable()
        {
            PersistenceSpecification<PersistenceCheckTarget>.BuildCheck(x => x.Number)
                .ShouldBeOfType<AccessorPersistenceCheck>()
                .Accessor.Name.ShouldEqual("Number");
        }

        [Test]
        public void pure_happy_path_1()
        {
            specification.Check(x => x.Names, x => x.Number, x => x.Description);
            specification.Verify();
        }

        [Test]
        public void pure_happy_path_2()
        {
            specification.Check(x => x.Names, x => x.Number, x => x.Description);
            original.Names = persisted.Names = new[]{"Jeremy", "Max"};
            original.Number = persisted.Number = 2;
            original.Description = persisted.Description = "something";

            specification.Verify();
        }

        [Test]
        public void smoke_test_of_failure()
        {
            specification.Check(x => x.Names, x => x.Number, x => x.Description);

            original.Number = 2;
            persisted.Number = 3;

            Exception<AssertionException>.ShouldBeThrownBy(() =>
            {
                specification.Verify();
            });
        }

        [Test]
        public void un_happy_path_with_one_difference()
        {
            specification.Check(x => x.Names, x => x.Number, x => x.Description);

            original.Number = 2;
            persisted.Number = 3;

            specification.GetMessages().Single().ShouldEqual("Number:  original was 2, but the persisted value was 3");
        }

        [Test]
        public void un_happy_path_with_multiple_failures()
        {
            specification.Check(x => x.Names, x => x.Number, x => x.Description);

            original.Number = 2;
            persisted.Number = 3;

            original.Names = new[]{"Jeremy", "Max"};

            specification.GetMessages().ShouldHaveTheSameElementsAs("Names:  original was Jeremy, Max, but the persisted value was null", "Number:  original was 2, but the persisted value was 3");
        }
    }

    public class PersistenceCheckTarget
    {
        public IEnumerable<string> Names { get; set; }
        public int? Number { get; set; }
        public string Description { get; set; }
    }
}