using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class BasicExtensionsTester
    {
         
        [Test]
        public void should_be_able_to_handle_null_targets()
        {
            Tester t = null;
            t.IfNotNull(te => te.StringValue).ShouldEqual(null);

        }

        [Test]
        public void should_be_able_to_handle_value_types()
        {
            Tester t = null;
            t.IfNotNull(te => te.Value).ShouldEqual(0);

        }

        [Test]
        public void should_not_call_on_null_objects()
        {
            Tester t = null;
            t.CallIfNotNull(te => te.Call());
        }

        [Test]
        public void should_call_on_instantiated_objects()
        {
            var t = new Tester();
            t.CallIfNotNull(te => te.Call());
            t.Called.ShouldBeTrue();
        }

        class Tester
        {
            public bool Called;
            public int Value { get; set; }
            public string StringValue { get; set; }
            public void Call()
            {
                Called = true;
            }
        }
    }
}