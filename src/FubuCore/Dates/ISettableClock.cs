using System;

namespace FubuCore.Dates
{
    public interface ISettableClock : ISystemTime
    {
        SystemTime Now(DateTime now);
    }
}