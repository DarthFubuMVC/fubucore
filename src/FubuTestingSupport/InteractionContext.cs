using System;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Logging;
using NUnit.Framework;
using StructureMap;
using StructureMap.AutoMocking;

namespace FubuTestingSupport
{
    public class InteractionContext<T> where T : class
    {
        private readonly Func<AutoMocker<T>> _autoMockFactory;
        private SettableClock _clock;

        public InteractionContext(Func<AutoMocker<T>> autoMockFactory)
        {
            _autoMockFactory = autoMockFactory;
        }

        public IContainer Container { get { return Services.Container; } }
        public AutoMocker<T> Services { get; private set; }
        public T ClassUnderTest { get { return Services.ClassUnderTest; } }

        [SetUp]
        public void SetUp()
        {
            _clock = new SettableClock();

            Services = _autoMockFactory();
            Services.Inject<ISystemTime>(_clock);
            beforeEach();
        }

        public RecordingLogger RecordLogging()
        {
            var logger = new RecordingLogger();
            Services.Inject<ILogger>(logger);

            return logger;
        }

        public RecordingLogger RecordedLog()
        {
            return MockFor<ILogger>().As<RecordingLogger>();
        }

        // Override this for context specific setup
        protected virtual void beforeEach() {}

        public TService MockFor<TService>() where TService : class
        {
            return Services.Get<TService>();
        }
        
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
    }
}
