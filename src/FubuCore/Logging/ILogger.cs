using System;

namespace FubuCore.Logging
{
    public interface ILogger
    {
        void Debug(string message, params object[] parameters);
        void Info(string message, params object[] parameters);
        void Error(string message, Exception ex);
        void Error(object correlationId, string message, Exception ex);

        void Debug(Func<string> message);
        void Info(Func<string> message);

        void DebugMessage(LogRecord message);
        void InfoMessage(LogRecord message);

        void DebugMessage<T>(Func<T> message) where T : LogRecord;
        void InfoMessage<T>(Func<T> message) where T : LogRecord;
    }
}