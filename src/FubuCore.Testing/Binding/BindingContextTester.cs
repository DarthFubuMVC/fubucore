using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BindingContextTester
    {
        [Test]
        public void creating_a_prefixed_context_brings_the_logger_with_it()
        {
            var context = new InMemoryBindingContext();
            var prefixed = context.PrefixWith("something");

            prefixed.Logger.ShouldBeTheSameAs(context.Logger);
        }
    }
}