using System;

namespace FubuCore.Logging
{
    /// <summary>
    /// Just what it says, provides a LogListener that writes to the
    /// Console
    /// </summary>
    public class ConsoleListener : FilteredListener<ConsoleListener>,ILogListener
    {
        public ConsoleListener(Level level) : base(level)
        {
        }

        protected override ConsoleListener thisInstance()
        {
            return this;
        }

        public void DebugMessage(object message)
        {
            Console.WriteLine(message);
        }

        public void InfoMessage(object message)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message, Exception ex)
        {
            Console.WriteLine(message);
            Console.WriteLine(ex);
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            Console.WriteLine(correlationId);
            Error(message, ex);
        }
    }
}