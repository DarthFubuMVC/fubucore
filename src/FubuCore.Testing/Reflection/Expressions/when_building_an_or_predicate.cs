using System.Collections.Generic;
using FubuCore.Reflection.Expressions;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Expressions
{
    [TestFixture]
    public class when_building_an_or_predicate
    {
        [Test]
        public void should_work()
        {
            var orish = new OrOperation().GetPredicateBuilder<Contract>(c => c.Status, "open", c=>c.Status, "closed");

            var contract = new Contract();
            contract.Status = "open";

            orish.Compile()(contract).ShouldBeTrue();

            var contract2 = new Contract();
            contract2.Status = "closed";


            orish.Compile()(contract2).ShouldBeTrue();
        }

        [Test]
        public void should_not_work()
        {
            var orish = new OrOperation().GetPredicateBuilder<Contract>(c => c.Status, "open", c => c.Status, "closed");

            var contract = new Contract();
            contract.Status = "a";

            orish.Compile()(contract).ShouldBeFalse();
        }

        [Test]
        public void should_work_for_collections()
        {
            var orish = new OrOperation().GetPredicateBuilder<Contract>(c => c.Status, "x", c => c.Status, new List<string>{"open","closed"});

            var contract = new Contract();
            contract.Status = "open";

            orish.Compile()(contract).ShouldBeTrue();
        }
    }
}