using FubuCore.Binding;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ObjectResolverTester : InteractionContext<ObjectResolver>
    {
        [Test]
        public void should_log_model_binder_selection()
        {
            var theModelBinder = MockFor<IModelBinder>();
            MockFor<IModelBinderCache>().Stub(x => x.BinderFor(typeof (Something)))
                .Return(theModelBinder);

            var data = new InMemoryRequestData();

            ClassUnderTest.BindModel(typeof (Something), data);

            MockFor<IBindingLogger>().AssertWasCalled(x => x.ChoseModelBinder(typeof(Something), theModelBinder));
        }


        public class Something{}
    }
}