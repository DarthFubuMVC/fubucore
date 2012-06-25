using System;
using FubuCore.Conversion;

namespace FubuCore.Dates
{
    public class Period
    {
        // For serialization
        public Period()
        {
        }

        public Period(DateTime @from)
        {
            From = from;
        }

        public Period(DateTime @from, DateTime? to)
        {
            From = from;
            To = to;
        }

        public DateTime From { get; set; }
        public DateTime? To { get; set; }

        public void MarkCompleted(DateTime completedTime)
        {
            To = completedTime;
        }

        public override string ToString()
        {
            return "{0} - {1}".ToFormat(From, To);
        }

        public bool IsActiveAt(DateTime timestamp)
        {
            if (timestamp < From) return false;

            return To.HasValue
                       ? timestamp < To.Value
                       : true;
        }

        public bool Equals(Period other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.From.Equals(From) && other.To.Equals(To);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Period)) return false;
            return Equals((Period)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (From.GetHashCode() * 397) ^ (To.HasValue ? To.Value.GetHashCode() : 0);
            }
        }

        public DateTime FindDateTime(TimeSpan time)
        {
            if (!To.HasValue)
            {
                throw new InvalidOperationException("FindDateTime can only be used if there is a value for To");
            }

            var date = From.Date;
            while (date <= To.Value)
            {
                var candidate = date.Add(time);
                if (IsActiveAt(candidate)) return candidate;

                date = date.AddDays(1);
            }

            throw new InvalidOperationException("Unable to find the matching time");
        }

        public DateTime FindDateTime(string timeString)
        {
            return FindDateTime(TimeSpanConverter.GetTimeSpan(timeString));
        }
    }
}