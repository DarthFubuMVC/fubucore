using System;
using System.Collections.Generic;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuCore.Util;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using Rhino.Mocks;

namespace FubuCore.Testing.Logging
{
    [TestFixture]
    public class LoggerTester
    {
        private readonly IEnumerable<ILogModifier> NulloModifiers = Enumerable.Empty<ILogModifier>();

        [Test]
        public void logger_will_not_throw_an_exception_if_one_listener_blows_chunks_for_error()
        {
            var l1 = MockRepository.GenerateMock<ILogListener>();
            var l2 = MockRepository.GenerateMock<ILogListener>();
            var l3 = MockRepository.GenerateMock<ILogListener>();

            

            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, NulloModifiers);

            var ex = new NotImplementedException();

            l2.Expect(x => x.Error("some message", ex)).Throw(new NotSupportedException());

            logger.Error("some message", ex);

            l1.AssertWasCalled(x => x.Error("some message", ex));
            l2.AssertWasCalled(x => x.Error("some message", ex));
            l3.AssertWasCalled(x => x.Error("some message", ex));
        }

        [Test]
        public void logger_will_not_throw_an_exception_if_one_listener_blows_chunks_for_debug()
        {
            var l1 = MockRepository.GenerateMock<ILogListener>();
            var l2 = MockRepository.GenerateMock<ILogListener>();
            var l3 = MockRepository.GenerateMock<ILogListener>();

            l1.Stub(x => x.IsDebugEnabled).Return(true);
            l2.Stub(x => x.IsDebugEnabled).Return(true);
            l3.Stub(x => x.IsDebugEnabled).Return(true);

            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, NulloModifiers);

            var ex = new NotImplementedException();

            l2.Expect(x => x.Debug("some message")).Throw(new NotSupportedException());

            logger.Debug("some message");

            l1.AssertWasCalled(x => x.Debug("some message"));
            l2.AssertWasCalled(x => x.Debug("some message"));
            l3.AssertWasCalled(x => x.Debug("some message"));
        }

        [Test]
        public void logger_will_not_throw_an_exception_if_one_listener_blows_chunks_for_info()
        {
            var l1 = MockRepository.GenerateMock<ILogListener>();
            var l2 = MockRepository.GenerateMock<ILogListener>();
            var l3 = MockRepository.GenerateMock<ILogListener>();

            l1.Stub(x => x.IsInfoEnabled).Return(true);
            l2.Stub(x => x.IsInfoEnabled).Return(true);
            l3.Stub(x => x.IsInfoEnabled).Return(true);

            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, NulloModifiers);

            var ex = new NotImplementedException();

            l2.Expect(x => x.Info("some message")).Throw(new NotSupportedException());

            logger.Info("some message");

            l1.AssertWasCalled(x => x.Info("some message"));
            l2.AssertWasCalled(x => x.Info("some message"));
            l3.AssertWasCalled(x => x.Info("some message"));
        }

        [Test]
        public void error_just_delegates_to_all_listeners()
        {
            var l1 = MockRepository.GenerateMock<ILogListener>();
            var l2 = MockRepository.GenerateMock<ILogListener>();
            var l3 = MockRepository.GenerateMock<ILogListener>();

            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, NulloModifiers);

            var ex = new NotImplementedException();
            logger.Error("some message", ex);

            l1.AssertWasCalled(x => x.Error("some message", ex));
            l2.AssertWasCalled(x => x.Error("some message", ex));
            l3.AssertWasCalled(x => x.Error("some message", ex));
        }

        [Test]
        public void error_just_delegates_to_all_listeners_2()
        {
            var l1 = MockRepository.GenerateMock<ILogListener>();
            var l2 = MockRepository.GenerateMock<ILogListener>();
            var l3 = MockRepository.GenerateMock<ILogListener>();

            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, NulloModifiers);

            var ex = new NotImplementedException();
            var correlationId = Guid.NewGuid();
            logger.Error(correlationId, "some message", ex);

            l1.AssertWasCalled(x => x.Error(correlationId, "some message", ex));
            l2.AssertWasCalled(x => x.Error(correlationId, "some message", ex));
            l3.AssertWasCalled(x => x.Error(correlationId, "some message", ex));
        }


        [Test]
        public void debug_with_mixed_listeners()
        {
            var l1 = new RecordingLogListener { IsDebugEnabled = true };
            var l2 = new RecordingLogListener { IsDebugEnabled = false, IsInfoEnabled = true };
            var l3 = new RecordingLogListener { IsDebugEnabled = true };

            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, NulloModifiers);

            logger.Debug("message {0}", 1);
            logger.Debug("message {0}", 2);
            logger.Debug(() => "message 3");

            l1.DebugStrings.ShouldHaveTheSameElementsAs("message 1", "message 2", "message 3");
            l3.DebugStrings.ShouldHaveTheSameElementsAs("message 1", "message 2", "message 3");

            l2.DebugStrings.Any().ShouldBeFalse();
        }

        [Test]
        public void info_with_mixed_listeners()
        {
            var l1 = new RecordingLogListener { IsInfoEnabled = true };
            var l2 = new RecordingLogListener { IsInfoEnabled = false, IsDebugEnabled = true };
            var l3 = new RecordingLogListener { IsInfoEnabled = true };

            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, NulloModifiers);

            logger.Info("message {0}", 1);
            logger.Info("message {0}", 2);
            logger.Info(() => "message 3");

            l1.InfoStrings.ShouldHaveTheSameElementsAs("message 1", "message 2", "message 3");
            l3.InfoStrings.ShouldHaveTheSameElementsAs("message 1", "message 2", "message 3");

            l2.InfoStrings.Any().ShouldBeFalse();
        }

        [Test]
        public void debugging_by_object()
        {
            var l1 = new RecordingLogListener { IsInfoEnabled = true };
            var l2 = new RecordingLogListener { IsInfoEnabled = false, IsDebugEnabled = true };
            var l3 = new RecordingLogListener { IsInfoEnabled = true };

            l1.ListensForTypes[typeof(Trace1)] = true;
            l2.ListensForTypes[typeof(Trace1)] = true;
            l3.ListensForTypes[typeof(Trace2)] = true;

            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, NulloModifiers);

            var msg1 = new Trace1();
            var msg2 = new Trace1();

            logger.DebugMessage(msg1);
            logger.DebugMessage(() => msg2);

            l1.DebugMessages.Any().ShouldBeFalse(); // debugging is not enabled
            l2.DebugMessages.ShouldHaveTheSameElementsAs(msg1, msg2);
            l3.DebugMessages.Any().ShouldBeFalse(); // does not listen to Trace1
        }

        [Test]
        public void debugging_by_message_puts_a_time_stamp_on_it()
        {
            var l1 = new RecordingLogListener { IsInfoEnabled = true };
            var l2 = new RecordingLogListener { IsInfoEnabled = false, IsDebugEnabled = true };
            var l3 = new RecordingLogListener { IsInfoEnabled = true };

            l1.ListensForTypes[typeof(StringMessage)] = true;
            l2.ListensForTypes[typeof(StringMessage)] = true;
            l3.ListensForTypes[typeof(StringMessage)] = true;

            var systemTime = SystemTime.AtLocalTime(DateTime.Today.AddHours(8));
            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, new ILogModifier[]{new LogRecordModifier(systemTime)});

            var message = new StringMessage("something");
            logger.DebugMessage(() => message);

            message.Time.ShouldEqual(systemTime.UtcNow());
        }

        [Test]
        public void info_by_message_puts_a_time_stamp_on_it()
        {
            var l1 = new RecordingLogListener { IsInfoEnabled = true };
            var l2 = new RecordingLogListener { IsInfoEnabled = false, IsDebugEnabled = true };
            var l3 = new RecordingLogListener { IsInfoEnabled = true };

            l1.ListensForTypes[typeof(StringMessage)] = true;
            l2.ListensForTypes[typeof(StringMessage)] = true;
            l3.ListensForTypes[typeof(StringMessage)] = true;

            var systemTime = SystemTime.AtLocalTime(DateTime.Today.AddHours(8));
            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, new ILogModifier[] { new LogRecordModifier(systemTime) });

            var message = new StringMessage("something");
            logger.InfoMessage(() => message);

            message.Time.ShouldEqual(systemTime.UtcNow());
        }

        [Test]
        public void debugging_by_message_puts_a_time_stamp_on_it_2()
        {
            var l1 = new RecordingLogListener { IsInfoEnabled = true };
            var l2 = new RecordingLogListener { IsInfoEnabled = false, IsDebugEnabled = true };
            var l3 = new RecordingLogListener { IsInfoEnabled = true };

            l1.ListensForTypes[typeof(StringMessage)] = true;
            l2.ListensForTypes[typeof(StringMessage)] = true;
            l3.ListensForTypes[typeof(StringMessage)] = true;

            var systemTime = SystemTime.AtLocalTime(DateTime.Today.AddHours(8));
            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, new ILogModifier[] { new LogRecordModifier(systemTime) });

            var message = new StringMessage("something");
            logger.DebugMessage(message);

            message.Time.ShouldEqual(systemTime.UtcNow());
        }

        [Test]
        public void info_by_message_puts_a_time_stamp_on_it_2()
        {
            var l1 = new RecordingLogListener { IsInfoEnabled = true };
            var l2 = new RecordingLogListener { IsInfoEnabled = false, IsDebugEnabled = true };
            var l3 = new RecordingLogListener { IsInfoEnabled = true };

            l1.ListensForTypes[typeof(StringMessage)] = true;
            l2.ListensForTypes[typeof(StringMessage)] = true;
            l3.ListensForTypes[typeof(StringMessage)] = true;

            var systemTime = SystemTime.AtLocalTime(DateTime.Today.AddHours(8));
            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, new ILogModifier[] { new LogRecordModifier(systemTime) });

            var message = new StringMessage("something");
            logger.InfoMessage(message);

            message.Time.ShouldEqual(systemTime.UtcNow());
        }


        [Test]
        public void info_by_object()
        {
            var l1 = new RecordingLogListener { IsInfoEnabled = false, IsDebugEnabled = true };
            var l2 = new RecordingLogListener { IsDebugEnabled = false, IsInfoEnabled = true };
            var l3 = new RecordingLogListener { IsInfoEnabled = true };

            l1.ListensForTypes[typeof(Trace1)] = true;
            l2.ListensForTypes[typeof(Trace1)] = true;
            l3.ListensForTypes[typeof(Trace2)] = true;

            var logger = new Logger(new ILogListener[] { l1, l2, l3 }, NulloModifiers);

            var msg1 = new Trace1();
            var msg2 = new Trace1();

            logger.InfoMessage(msg1);
            logger.InfoMessage(() => msg2);

            l1.InfoMessages.Any().ShouldBeFalse(); // debugging is not enabled
            l2.InfoMessages.ShouldHaveTheSameElementsAs(msg1, msg2);
            l3.InfoMessages.Any().ShouldBeFalse(); // does not listen to Trace1
        }

        public class Trace1 : LogRecord { }
        public class Trace2 : LogRecord { }
        public class Trace3 : LogRecord { }
    }

    public class RecordingLogListener : ILogListener
    {
        public bool IsDebugEnabled
        {
            get;
            set;
        }

        public bool IsInfoEnabled
        {
            get;
            set;
        }

        public readonly Cache<Type, bool> ListensForTypes = new Cache<Type, bool>(t => false);

        public bool ListensFor(Type type)
        {
            return ListensForTypes[type];
        }

        public readonly IList<object> DebugMessages = new List<object>();
        public void DebugMessage(object message)
        {
            DebugMessages.Add(message);
        }

        public readonly IList<object> InfoMessages = new List<object>();
        public void InfoMessage(object message)
        {
            InfoMessages.Add(message);
        }

        public readonly IList<string> DebugStrings = new List<string>();
        public void Debug(string message)
        {
            DebugStrings.Add(message);
        }

        public readonly IList<string> InfoStrings = new List<string>();
        public void Info(string message)
        {
            InfoStrings.Add(message);
        }

        public void Error(string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}