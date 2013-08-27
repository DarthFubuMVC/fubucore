using System;
using System.Linq;
using FubuCore.Logging;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Logging
{
    [TestFixture]
    public class RecordingLoggerTester
    {
        [Test]
        public void start_with_empty_logs()
        {
            var logger = new RecordingLogger();
            logger.DebugMessages.ShouldHaveCount(0);
            logger.InfoMessages.ShouldHaveCount(0);
            logger.ErrorMessages.ShouldHaveCount(0);
        }

        [Test]
        public void log_error_should_be_recorded()
        {
            var logger = new RecordingLogger();
            logger.Error("Error logged", new InvalidOperationException("blah"));
            var logEntry = logger.ErrorMessages.OfType<ExceptionReport>().First();
            logEntry.Message.ShouldEqual("Error logged");
            logEntry.ExceptionType.ShouldEqual(typeof(InvalidOperationException).Name);
        }

        [Test]
        public void log_debug_message_should_be_formatted()
        {
            var logger = new RecordingLogger();
            logger.Debug("Fun times {0}", "at ridgemont high");
            var logEntry = logger.DebugMessages.OfType<StringMessage>().First();
            logEntry.Message.ShouldEqual("Fun times at ridgemont high");
        }

        [Test]
        public void log_info_with_func_message_should_log()
        {
            var logger = new RecordingLogger();
            logger.Info(() => "Stale information");
            var logEntry = logger.InfoMessages.OfType<StringMessage>().First();
            logEntry.Message.ShouldEqual("Stale information");
        }
    }
}