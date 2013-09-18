using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Logging
{
    public class RecordingLogger : ILogger
    {
        private readonly object _lock = new object();
        private readonly IList<object> _debug = new List<object>();
        private readonly IList<object> _info = new List<object>();
        private readonly IList<ExceptionReport> _errors = new List<ExceptionReport>();

        public IEnumerable<object> DebugMessages
        {
            get
            {
                lock (_lock)
                {
                    return _debug.ToArray();
                }
            }
        }

        public IEnumerable<object> InfoMessages
        {
            get
            {
                lock (_lock)
                {
                    return _info.ToArray();
                }
            }
        }

        public IEnumerable<LogRecord> ErrorMessages
        {
            get
            {
                lock (_lock)
                {
                    return _errors.ToArray();
                }
            }
        }

        public void Debug(string message, params object[] parameters)
        {
            lock (_lock)
            {
                _debug.Add(new StringMessage(message, parameters));
            }
        }

        public void Info(string message, params object[] parameters)
        {
            lock (_lock)
            {
                _info.Add(new StringMessage(message, parameters));
            }
        }

        public void Error(string message, Exception ex)
        {
            lock (_lock)
            {
                _errors.Add(new ExceptionReport(message, ex));
            }
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            lock (_lock)
            {
                _errors.Add(new ExceptionReport
                {
                    Message = message,
                    ExceptionText = ex.ToString(),
                    CorrelationId = correlationId
                });
            }
        }

        public void Debug(Func<string> message)
        {
            Debug(message());
        }

        public void Info(Func<string> message)
        {
            Info(message());
        }

        public void DebugMessage(LogTopic message)
        {
            lock (_lock)
            {
                _debug.Add(message);
            }
        }

        public void InfoMessage(LogTopic message)
        {
            lock (_lock)
            {
                _info.Add(message);
            }
        }

        public void DebugMessage<T>(Func<T> message) where T : class, LogTopic
        {
            lock (_lock)
            {
                _debug.Add(message());
            }
        }

        public void InfoMessage<T>(Func<T> message) where T : class, LogTopic
        {
            lock (_lock)
            {
                _info.Add(message());
            }
        }

        public void DebugMessage(LogRecord message)
        {
            lock (_lock)
            {
                _debug.Add(message);
            }
        }

        public void InfoMessage(LogRecord message)
        {
            lock (_lock)
            {
                _info.Add(message);
            }
        }

    }
}