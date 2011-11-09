using System;
using System.Collections.Generic;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuLocalization.Tests
{
    [TestFixture]
    public class LocalStringTester
    {
        [Test]
        public void two_instances_with_the_same_key_should_equal_each_other()
        {
            var x = new LocalString { value = "foo" };
            var y = new LocalString { value = "foo" };

            x.ShouldEqual(y);
            y.ShouldEqual(x);
        }

        [Test]
        public void two_instances_with_the_same_key_should_be_considered_the_same_for_hashing_purposes()
        {
            var x = new LocalString { value = "foo" };
            var y = new LocalString { value = "foo" };

            var dict = new Dictionary<LocalString, int> { { x, 0 } };

            dict.ContainsKey(y).ShouldBeTrue();
        }

        [Test]
        public void read_from_happy_path()
        {
            LocalString.ReadFrom("key=foo").ShouldEqual(new LocalString("key", "foo"));
            
        }


        [Test]
        public void read_from_happy_path_trims()
        {
            LocalString.ReadFrom("     key=foo   ").ShouldEqual(new LocalString("key", "foo"));

        }

        [Test]
        public void read_from_sad_path_fails_with_descriptive_error()
        {
            Exception<ArgumentException>.ShouldBeThrownBy(() =>
            {
                LocalString.ReadFrom("key:foo");
            }).Message.ShouldEqual("LocalString must be expressed as 'value=display', 'key:foo' is invalid");

            
        }

        [Test]
        public void read_all()
        {
            LocalString.ReadAllFrom(@"
a=a-display
b=b-display
c=c-display
").ShouldHaveTheSameElementsAs(new LocalString("a", "a-display"), new LocalString("b", "b-display"), new LocalString("c", "c-display"));
        }
    }
}