using System;
using FubuTestingSupport;
using NSubstitute;
using StructureMap.AutoMocking;

namespace FubuCore.Testing
{
    public class NSubstituteAutoMocker<T> : AutoMocker<T> where T : class
    {
        public NSubstituteAutoMocker() : base(new NSubstituteServiceLocator())
        {
        }
    }

    public class NSubstituteServiceLocator : ServiceLocator
    {
        public T Service<T>() where T : class
        {
            return Substitute.For<T>();
        }

        public object Service(Type serviceType)
        {
            return Substitute.For(new [] { serviceType }, new object [] {});
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            return Substitute.For<T>(args);
        }
    }

    public class NSubstituteInteractionContext<T> : InteractionContext<T> where T : class
    {
        public NSubstituteInteractionContext() : base(() => new NSubstituteAutoMocker<T>())
        {
        }
    }
}