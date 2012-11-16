using System;

namespace FubuCore.CommandLine
{
    [Serializable]
    public class InvalidUsageException : Exception
    {
        public InvalidUsageException() : base(string.Empty) {}

        public InvalidUsageException(string message) : base(message)
        {
            
        }

        public InvalidUsageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}