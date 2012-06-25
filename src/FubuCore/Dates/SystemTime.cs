using System;

namespace FubuCore.Dates
{
    public class SystemTime : ISystemTime, ISettableClock
    {
        private Func<DateTime> _now;

        public SystemTime()
        {
            _now = () => DateTime.Now;
        }

        public DateTime Now()
        {
            // TODO -- Need to go UTC sooner rather than later
            return _now();
        }

        public void Live()
        {
            _now = () => DateTime.Now;
        }

        public SystemTime Now(DateTime now)
        {
            _now = () => now;
            return this;
        }

        public SystemTime Now(Func<DateTime> now)
        {
            _now = now;

            return this;
        }

        public SystemTime RestartAt(DateTime desiredTime)
        {
            var delta = desiredTime.Subtract(DateTime.Now);
            Now(() => DateTime.Now.Add(delta));

            return this;
        }

        public Date Today()
        {
            return new Date(Now().Date);
        }

        public TimeSpan CurrentTime()
        {
            return Now().TimeOfDay;
        }
    }
}