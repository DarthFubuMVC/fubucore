using System.Collections.Generic;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.TestingSupport
{
    [TestFixture]
    public class EnumerablePersistenceCheckTester
    {
        private EnumerablePersistenceCheck<string> check;
        private EnumerableCheckTarget original;
        private EnumerableCheckTarget persisted;


        [SetUp]
        public void SetUp()
        {
            check = EnumerablePersistenceCheck<string>.For<EnumerableCheckTarget>(x => x.Names);
            original = new EnumerableCheckTarget();
            persisted = new EnumerableCheckTarget();
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
            original.Names = null;
            persisted.Names = new string[]{"Jeremy", "Max"};

            theMessage.ShouldEqual("Names:  original was null, but the persisted value was Jeremy, Max");
        }

        [Test]
        public void persisted_is_null_but_original_was_not()
        {
            original.Names = new string[] { "Jeremy", "Max" }; ;
            persisted.Names = null;

            theMessage.ShouldEqual("Names:  original was Jeremy, Max, but the persisted value was null");
        }

        [Test]
        public void values_are_different()
        {
            original.Names = new string[] { "Jeremy", "Max" }; ;
            persisted.Names = new string[] { "Lindsey" }; ;

            theMessage.ShouldEqual("Names:  original was Jeremy, Max, but the persisted value was Lindsey");
        }

        [Test]
        public void values_are_different_with_same_length_of_names()
        {
            original.Names = new string[] { "Jeremy", "Max" }; ;
            persisted.Names = new string[] { "Lindsey", "Max" }; ;

            theMessage.ShouldEqual("Names:  original was Jeremy, Max, but the persisted value was Lindsey, Max");
        }

        [Test]
        public void happy_path_adds_no_messages()
        {
            original.Names = new string[] { "Jeremy", "Max" };
            persisted.Names = new string[] { "Jeremy", "Max" };

            var list = new List<string>();
            check.CheckValue(original, persisted, list.Add);

            list.Any().ShouldBeFalse();
        }

        [Test]
        public void happy_path_with_both_null()
        {
            original.Names = null;
            persisted.Names = null;

            var list = new List<string>();
            check.CheckValue(original, persisted, list.Add);

            list.Any().ShouldBeFalse();
        }


    }

    public class EnumerableCheckTarget
    {
        public IEnumerable<string> Names { get; set; }
    }
}