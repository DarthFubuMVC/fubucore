using System;

namespace FubuCore.Dates
{
    public interface ISystemTime
    {
        DateTime UtcNow();

        LocalTime LocalTime();
    }
}