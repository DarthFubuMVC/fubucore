using FubuCore.Binding;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [MarkedForTermination("Convert all to the BindingScenario")]
    public class InMemoryBindingContext : BindingContext
    {
        private readonly InMemoryRequestData _data;
        private readonly InMemoryServiceLocator _services;

        public InMemoryBindingContext()
            : this(new InMemoryRequestData(), new InMemoryServiceLocator(), MockRepository.GenerateMock<IBindingLogger>())
        {
        }

        private InMemoryBindingContext(InMemoryRequestData data, InMemoryServiceLocator services)
            : base(data, services, new NulloBindingLogger())
        {
            _services = services;
            _data = data;
        }

        private InMemoryBindingContext(InMemoryRequestData data, InMemoryServiceLocator services, IBindingLogger logger)
            : base(data, services, logger)
        {
            _services = services;
            _data = data;
        }

        public InMemoryRequestData Data
        {
            get { return _data; }
        }

        public object this[string key]
        {
            get { return _data[key]; }
            set { _data[key] = value; }
        }


        public void RegisterService<T>(T @object)
        {
            _services.Add(@object);
        }

        public InMemoryBindingContext WithData(string key, object value)
        {
            this[key] = value;
            return this;
        }

        public InMemoryBindingContext WithPropertyValue(object value)
        {
            PropertyValue = value;
            return this;
        }
    }
}