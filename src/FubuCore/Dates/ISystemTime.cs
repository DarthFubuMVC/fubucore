using System;

namespace FubuCore.Dates
{
    public interface ISystemTime
    {
        DateTime LocalNow();
        Date LocalDay();
        TimeSpan LocalTime();
        DateTime UtcNow();
    }
}