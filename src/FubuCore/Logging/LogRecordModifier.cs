using System;
using FubuCore.Dates;

namespace FubuCore.Logging
{
    public class LogRecordModifier : ILogModifier
    {
        private readonly ISystemTime _systemTime;

        public LogRecordModifier(ISystemTime systemTime)
        {
            _systemTime = systemTime;
        }

        public bool Matches(Type logType)
        {
            return logType.IsConcreteTypeOf<LogRecord>();
        }

        public void Modify(object log)
        {
            var record = log.As<LogRecord>();

            record.Time = _systemTime.UtcNow();
        }
    }
}