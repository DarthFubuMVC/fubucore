using System;

namespace FubuCore.Dates
{
    public interface ISystemTime
    {
        DateTime Now();
        Date Today();
        TimeSpan CurrentTime();
    }
}