using System;
using System.Collections.Generic;
using FubuCore.Dates;
using FubuCore.Util;

namespace FubuCore.Logging
{
    public class Logger : ILogger
    {
        private readonly ISystemTime _systemTime;
        private readonly ListenerCollection _listeners;
        private readonly Lazy<Action<Func<string>>> _debugString;
        private readonly Lazy<Action<Func<string>>> _infoString;
        private readonly Cache<Type, Action<Func<object>>> _debugMessage = new Cache<Type, Action<Func<object>>>();
        private readonly Cache<Type, Action<Func<object>>> _infoMessage = new Cache<Type, Action<Func<object>>>();

        public Logger(ISystemTime systemTime, IEnumerable<ILogListener> listeners)
        {
            _systemTime = systemTime;
            _listeners = new ListenerCollection(listeners);
            _debugString = new Lazy<Action<Func<string>>>(() => _listeners.Debug());
            _infoString = new Lazy<Action<Func<string>>>(() => _listeners.Info());

            _debugMessage.OnMissing = type => _listeners.DebugFor(type);
            _infoMessage.OnMissing = type => _listeners.InfoFor(type);
        }

        public void Debug(string message, params object[] parameters)
        {
            Debug(() => message.ToFormat(parameters));
        }

        public void Info(string message, params object[] parameters)
        {
            Info(() => message.ToFormat(parameters));
        }

        public void Error(string message, Exception ex)
        {
            _listeners.Each(x =>
            {
                try
                {
                    x.Error(message, ex);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            _listeners.Each(x =>
            {
                try
                {
                    x.Error(correlationId, message, ex);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        public void Debug(Func<string> message)
        {
            _debugString.Value(message);
        }

        public void Info(Func<string> message)
        {
            _infoString.Value(message);
        }

        public void DebugMessage(LogRecord message)
        {
            if (message == null)
            {
                return;
            }


            _debugMessage[message.GetType()](wrapWithTime(() => message));
        }

        public void InfoMessage(LogRecord message)
        {
            if (message == null)
            {
                return;
            }

            _infoMessage[message.GetType()](wrapWithTime(() => message));
        }

        private Func<T> wrapWithTime<T>(Func<T> func) where T : LogRecord
        {
            return () =>
            {
                var log = func();
                log.Time = _systemTime.UtcNow();

                return log;
            };
        }

        public void DebugMessage<T>(Func<T> message) where T : LogRecord
        {
            Func<T> withTime = wrapWithTime(message);
            _debugMessage[typeof(T)](withTime);
        }

        public void InfoMessage<T>(Func<T> message) where T : LogRecord
        {
            Func<T> withTime = wrapWithTime(message);
            _infoMessage[typeof(T)](withTime);
        }
    }
}