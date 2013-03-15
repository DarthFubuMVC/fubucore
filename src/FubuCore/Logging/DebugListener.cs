using System;

namespace FubuCore.Logging
{
    public class DebugListener : FilteredListener<DebugListener>, ILogListener
    {
        public DebugListener(Level level) : base(level)
        {
        }

        protected override DebugListener thisInstance()
        {
            return this;
        }

        public void DebugMessage(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void InfoMessage(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Error(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(ex);
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(correlationId);
            Error(message, ex);
        }


    }
}