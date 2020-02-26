using System.Linq;
using FubuCore.Logging;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.TestingSupport
{
    [TestFixture]
    public class RecordingLoggerMechanicsInInteractionContext : NSubstituteInteractionContext<SomeClass>
    {
        [Test]
        public void can_inject_and_get_the_recording_logger()
        {
            var logger = RecordLogging();

            ClassUnderTest.Go("somewhere");

            var debugMessages = RecordedLog().DebugMessages;

            RecordedLog().ShouldBeTheSameAs(logger);

            debugMessages.Single().As<StringMessage>()
                .Message.ShouldEqual("somewhere");
        }
    }

    public class SomeClass
    {
        private readonly ILogger _logger;

        public SomeClass(ILogger logger)
        {
            _logger = logger;
        }

        public void Go(string trace)
        {
            _logger.Debug(trace);
        }
    }
}