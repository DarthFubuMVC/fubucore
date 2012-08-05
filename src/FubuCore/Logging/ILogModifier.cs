using System;

namespace FubuCore.Logging
{
    public interface ILogModifier
    {
        bool Matches(Type logType);
        void Modify(object log);
    }
}