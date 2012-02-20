using System;

namespace FubuCore.Binding
{
    [Serializable]
    public class BindingValue
    {
        public string RawKey { get; set; }
        public string Source { get; set; }
        public object RawValue { get; set; }

        public bool Equals(BindingValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.RawKey, RawKey) && Equals(other.Source, Source) && Equals(other.RawValue, RawValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BindingValue)) return false;
            return Equals((BindingValue) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (RawKey != null ? RawKey.GetHashCode() : 0);
                result = (result*397) ^ (Source != null ? Source.GetHashCode() : 0);
                result = (result*397) ^ (RawValue != null ? RawValue.GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("RawKey: {0}, Source: {1}, RawValue: {2}", RawKey, Source, RawValue);
        }
    }
}