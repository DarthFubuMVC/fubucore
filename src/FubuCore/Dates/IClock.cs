using System;

namespace FubuCore.Dates
{
    public interface IClock
    {
        DateTime UtcNow();
    }
}