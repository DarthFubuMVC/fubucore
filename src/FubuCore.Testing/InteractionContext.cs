using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using FubuCore.Dates;

namespace FubuCore.Testing
{
    public class InteractionContext<TClassUnderTest> where TClassUnderTest : class
    {
        static readonly Random RandomGenerator = new Random();

        protected AutoMocker Services { get; set; }

        SettableClock _clock;
        Lazy<TClassUnderTest> _lazyClassUnderTest;

        [SetUp]
        public void SetUp()
        {
            _clock = new SettableClock();
            Services = new AutoMocker(MockBehavior.Default, DefaultValue.Mock, true);
            Services.Inject<ISystemTime>(_clock);

            _lazyClassUnderTest = new Lazy<TClassUnderTest>(() =>
            {
                if (typeof(TClassUnderTest) == typeof(StaticClass))
                {
                    throw new InvalidOperationException($"Cannot initialize {nameof(ClassUnderTest)} when testing a static class.");
                }

                return Services.CreateSelfMock<TClassUnderTest>();
            });

            beforeEach();
        }

        // Override this for context specific setup
        protected virtual void beforeEach()
        {
        }

        public void VerifyCallsFor<MOCK>() where MOCK : class
        {
            MockFor<MOCK>().Verify();
        }

        public TClassUnderTest ClassUnderTest => _lazyClassUnderTest.Value;

        public Mock<TClassUnderTest> ClassUnderTestMock => ((IMocked<TClassUnderTest>) ClassUnderTest).Mock;

        public Mock<TService> MockFor<TService>() where TService : class 
            => Services.GetMock<TService>();

        protected int Random(int min = int.MinValue, int max = int.MaxValue) => RandomGenerator.Next(min, max);

        public DateTime LocalSystemTime
        {
            get
            {
                return _clock.LocalTime().Time;
            }
            set
            {
                _clock.LocalNow(value);
            }
        }

        public LocalTime LocalTime
        {
            get
            {
                return _clock.LocalTime();
            }
        }

        public DateTime UtcSystemTime
        {
            get
            {
                return _clock.UtcNow();
            }
        }

        class AutoRequestServices : IServiceProvider
        {
            readonly AutoMocker _autoMocker;

            public AutoRequestServices(AutoMocker autoMocker)
            {
                _autoMocker = autoMocker;
            }

            public object GetService(Type serviceType) => _autoMocker.Get(serviceType);
        }
    }

    /// <summary>
    ///  This is a filler type to plugin to the generic type
    /// of <see cref="InteractionContext{TClassUnderTest}"/> for testing static classes.
    /// </summary>
    public class StaticClass
    {
    }
}