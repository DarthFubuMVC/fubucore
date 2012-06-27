using System;

namespace FubuCore.Dates
{
    public interface ISettableClock : ISystemTime
    {
        SystemTime LocalNow(DateTime now);
    }
}