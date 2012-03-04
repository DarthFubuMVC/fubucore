using FubuCore.Formatting;
using FubuCore.Reflection;
using FubuCore.Testing.Conversion;
using FubuCore.Testing.Reflection.Expressions;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Formatting
{
    [TestFixture]
    public class GetStringRequestTester
    {
        [Test]
        public void when_creating_a_string_request_with_services_via_constructor()
        {
            var locator = MockRepository.GenerateMock<IServiceLocator>();
            var accessor = ReflectionHelper.GetAccessor<Case>(x => x.Contact.FirstName);

            var request = new GetStringRequest(accessor, "something", locator);

            request.PropertyType.ShouldEqual(typeof (string));
            request.OwnerType.ShouldEqual(typeof (Contact));


        }

        [Test]
        public void property_type_is_determined_from_the_prop_if_no_accessor()
        {
            var property = ReflectionHelper.GetProperty<Case>(x => x.Identifier);
            var request = new GetStringRequest(property, "something");

            request.OwnerType.ShouldEqual(typeof (Case));
            request.PropertyType.ShouldEqual(typeof (string));

        }
    }
}