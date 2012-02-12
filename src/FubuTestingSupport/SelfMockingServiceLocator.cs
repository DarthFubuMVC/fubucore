using System;
using FubuCore;
using FubuCore.Util;
using Rhino.Mocks;

namespace FubuTestingSupport
{
    public class SelfMockingServiceLocator : IServiceLocator
    {
        private readonly Cache<Type, object> _mocks = new Cache<Type, object>(t =>
        {
            var makerType = typeof (MockMaker<>).MakeGenericType(t);
            return Activator.CreateInstance(makerType).As<MockMaker>().Make();
        });

        public TService GetInstance<TService>()
        {
            return (TService) _mocks[typeof (TService)];
        }

        public T MockFor<T>()
        {
            return (T) _mocks[typeof (T)];
        }

        public object GetService(Type serviceType)
        {
            return _mocks[serviceType];
        }

        public object GetInstance(Type serviceType)
        {
            return _mocks[serviceType];
        }


        public void Stub<T>(T stub)
        {
            _mocks[typeof (T)] = stub;
        }

        #region Nested type: MockMaker

        public interface MockMaker
        {
            object Make();
        }

        public class MockMaker<T> : MockMaker where T : class
        {
            public object Make()
            {
                return MockRepository.GenerateMock<T>();
            }
        }

        #endregion
    }
}