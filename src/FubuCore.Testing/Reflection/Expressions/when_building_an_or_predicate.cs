using System;
using System.Collections.Generic;
using FubuCore.Reflection.Expressions;
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
            var orish = new OrOperation().GetPredicateBuilder<Contract>(c => c.Status, new List<string>{"open1","closed1"}, c=>c.Status, "open");

            var contract = new Contract();
            contract.Status = "open";

            orish.Compile()(contract).ShouldBeTrue();
        }
        [Test]
        public void should_work_for_collections_and_other()
        {
            var orish = new OrOperation().GetPredicateBuilder<Contract>( c => c.Status, new List<string>{"open","closed"}, c=>c.Status, "x");

            var contract = new Contract();
            contract.Status = "x";

            orish.Compile()(contract).ShouldBeTrue();
        }
    }



    [TestFixture]
    public class when_composing_an_or_predicate
    {
        [Test]
        public void should_work()
        {
            var orish = new ComposableOrOperation();//.GetPredicateBuilder<Contract>(, c => c.Status, "closed");
            orish.Set<Contract>(c => c.Status, "open");
            orish.Set<Contract>(c => c.Status, "closed");
            orish.Set<Contract>(c => c.Status, "hold");

            var x = orish.GetPredicateBuilder<Contract>();

            var contract = new Contract();
            contract.Status = "hold";

            x.Compile()(contract).ShouldBeTrue();

            var contract2 = new Contract();
            contract2.Status = "closed";


            x.Compile()(contract2).ShouldBeTrue();
        }

        [Test]
        public void should_work_with_zero_ors()
        {
            var orish = new ComposableOrOperation();

            Exception<Exception>.ShouldBeThrownBy(() =>
            {
                orish.GetPredicateBuilder<Contract>().Compile()(new Contract());
            });
        }

        [Test]
        public void should_not_work()
        {
            var orish = new ComposableOrOperation();//.GetPredicateBuilder<Contract>(, c => c.Status, "closed");
            orish.Set<Contract>(c => c.Status, "open");
            orish.Set<Contract>(c => c.Status, "closed");

            var x = orish.GetPredicateBuilder<Contract>();

            var contract = new Contract();
            contract.Status = "a";

            x.Compile()(contract).ShouldBeFalse();
        }

        [Test]
        public void should_work_for_collections()
        {

            var orish = new ComposableOrOperation();
            orish.Set<Contract>(c => c.Status, "closed");
            orish.Set<Contract>(c => c.Status, new List<string> { "open", "closed" });

            var x = orish.GetPredicateBuilder<Contract>();

            var contract = new Contract();
            contract.Status = "open";

            x.Compile()(contract).ShouldBeTrue();
        }

        [Test]
        public void should_work_for_collections_a()
        {

            var orish = new ComposableOrOperation();
            orish.Set<Contract>(c => c.Status, new List<string> { "open", "closed" });
            orish.Set<Contract>(c => c.Status, "closed");

            var x = orish.GetPredicateBuilder<Contract>();

            var contract = new Contract();
            contract.Status = "open";

            x.Compile()(contract).ShouldBeTrue();
        }

        [Test]
        public void should_work_for_path()
        {
            var orish = new ComposableOrOperation();
            orish.Set<Contract>(c => c.Part.IsUsed, true);
            orish.Set<Contract>(c => c.Status, new List<string> { "open", "closed" });

            var x = orish.GetPredicateBuilder<Contract>();

            var contract = new Contract();
            contract.Status = "opn";
            contract.Part.IsUsed = true;
            
            x.Compile()(contract).ShouldBeTrue();
        }

        [Test]
        public void should_work_for_non_primitive_collections()
        {
                var sigs  = new List<Signature>
                                  {
                                      new Signature("ryan"),
                                      new Signature("dru"),
                                      new Signature("brandon")
                                  };

            var orish = new ComposableOrOperation();
            orish.Set<Contract>(c => c.Part.IsUsed, true);
            orish.Set<Contract>(c => c.Signature, sigs);

            var x = orish.GetPredicateBuilder<Contract>();
            
            var contract = new Contract();
            contract.Part.IsUsed = false;
            contract.Signature = new Signature("brandon");
            
            x.Compile()(contract).ShouldBeTrue();
        }

        [Test]
        public void should_work_for_non_primitive_collections_with_equality_overloads()
        {
            var sigs = new List<Signature>
            {
                new ProxySignature("rex", "joe")
            };

            var orish = new ComposableOrOperation();
            orish.Set<Contract>(c => c.Part.IsUsed, true);
            orish.Set<Contract>(c => c.Signature, sigs);

            var x = orish.GetPredicateBuilder<Contract>();

            var contract = new Contract();
            contract.Part.IsUsed = false;
            contract.Signature = new Signature("rex");

            x.Compile()(contract).ShouldBeTrue();
        }

        [Test]
        public void should_work_for_two_where()
        {
            var orish = new ComposableOrOperation();
            orish.Set<Contract>(c => c.Part.IsUsed, true);
            orish.Set<Contract>(c => c.Purchased, 2);

            var x = orish.GetPredicateBuilder<Contract>();
            
            var contract = new Contract();
            contract.Status = "open";
            contract.Purchased = 2;
            contract.Part.IsUsed = false;
            contract.Signature = new Signature("brandon");
            contract.IsUnitBased = false;
            
            x.Compile()(contract).ShouldBeTrue();
        }
        
    }
}