using System.Collections.Generic;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuCore.Testing.TestingSupport
{
    [TestFixture]
    public class AccessorPersistenceCheckTester
    {
        private AccessorPersistenceCheck check;
        private CheckTarget original;
        private CheckTarget persisted;


        [SetUp]
        public void SetUp()
        {
            check = AccessorPersistenceCheck.For<CheckTarget>(x => x.Number);
            original = new CheckTarget();
            persisted = new CheckTarget();
        }

        private string theMessage
        {
            get
            {
                var list = new List<string>();
                check.CheckValue(original, persisted, list.Add);

                return list.Single();
            }
        }

        [Test]
        public void original_is_null_but_persisted_is_not()
        {
            original.Number = null;
            persisted.Number = 1;

            theMessage.ShouldEqual("Number:  original was null, but the persisted value was 1");
        }

        [Test]
        public void persisted_is_null_but_original_was_not()
        {
            original.Number = 1;
            persisted.Number = null;

            theMessage.ShouldEqual("Number:  original was 1, but the persisted value was null");
        }

        [Test]
        public void values_are_different()
        {
            original.Number = 1;
            persisted.Number = 2;

            theMessage.ShouldEqual("Number:  original was 1, but the persisted value was 2");
        }

        [Test]
        public void happy_path_adds_no_messages()
        {
            original.Number = 1;
            persisted.Number = 1;
            
            var list = new List<string>();
            check.CheckValue(original, persisted, list.Add);

            list.Any().ShouldBeFalse();
        }

        [Test]
        public void happy_path_with_both_null()
        {
            original.Number = null;
            persisted.Number = null;

            var list = new List<string>();
            check.CheckValue(original, persisted, list.Add);

            list.Any().ShouldBeFalse();
        }

        [Test]
        public void messages_with_string()
        {
            var stringCheck = AccessorPersistenceCheck.For<CheckTarget>(x => x.Name);
            original.Name = "Jeremy";
            persisted.Name = "Max";

            var list = new List<string>();
            stringCheck.CheckValue(original, persisted, list.Add);

            list.Single().ShouldEqual("Name:  original was 'Jeremy', but the persisted value was 'Max'");
        }
    }

    public class CheckTarget
    {
        public int? Number { get; set; }
        public string Name { get; set; }
    }
}