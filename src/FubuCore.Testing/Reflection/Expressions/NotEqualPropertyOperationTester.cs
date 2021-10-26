using FubuCore.Reflection.Expressions;
using NUnit.Framework;
using System;

namespace FubuCore.Testing.Reflection.Expressions
{
    public class NotEqualPropertyOperationTester {}

    [TestFixture]
    public class when_building_a_predicate_for_object_not_equality_with_operator_overload_on_base_class
    {
        private Func<Contract, bool> _builtPredicate;
        private string signatureName;

        [SetUp]
        public void Setup()
        {
            signatureName = Guid.NewGuid().ToString();
            var signature = new ProxySignature(signatureName, Guid.NewGuid().ToString());
            var builder = new NotEqualPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Contract>(c => c.Signature)(signature).Compile();
        }

        [Test]
        public void should_succeed_when_the_object_is_equal()
        {
            var contract = Contract.For("Open");
            contract.Signature = new ProxySignature(signatureName, Guid.NewGuid().ToString());
            _builtPredicate(contract).ShouldBeFalse();
        }

        [Test]
        public void should_not_succeed_when_the_object_is_not_equal()
        {
            var contract = Contract.For("Open");
            contract.Signature = new ProxySignature(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            _builtPredicate(contract).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_building_a_predicate_for_object_not_equality_with_operator_overload_on_object
    {
        private Func<Contract, bool> _builtPredicate;
        private string signatureName;

        [SetUp]
        public void Setup()
        {
            signatureName = Guid.NewGuid().ToString();
            var signature = new Signature(signatureName);
            var builder = new NotEqualPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Contract>(c => c.Signature)(signature).Compile();
        }

        [Test]
        public void should_succeed_when_the_object_is_equal()
        {
            var contract = Contract.For("Open");
            contract.Signature = new Signature(signatureName);
            _builtPredicate(contract).ShouldBeFalse();
        }

        [Test]
        public void should_not_succeed_when_the_object_is_not_equal()
        {
            var contract = Contract.For("Open");
            contract.Signature = new Signature(Guid.NewGuid().ToString());
            _builtPredicate(contract).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_building_a_predicate_for_valueobject_not_equality
    {
        private Func<Contract, bool> _builtPredicate;

        [SetUp]
        public void Setup()
        {
            var builder = new NotEqualPropertyOperation();
            _builtPredicate = builder.GetPredicateBuilder<Contract>(c => c.Status)("Open").Compile();
        }

        [Test]
        public void should_not_succeed_when_the_property_contains_the_exact_value()
        {
            var contract = new Contract();
            contract.Status = "Open";
            _builtPredicate(contract).ShouldBeFalse();
        }

        [Test]
        public void should_succeed_when_the_property_contains_a_different_value()
        {
            var contract = new Contract();
            contract.Status = "Closed";
            _builtPredicate(contract).ShouldBeTrue();
        }
    }
}