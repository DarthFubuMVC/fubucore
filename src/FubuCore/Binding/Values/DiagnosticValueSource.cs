namespace FubuCore.Binding.Values
{
    public class DiagnosticValueSource
    {
        public string Source { get; set; }
        public string Value { get; set; }

        public DiagnosticValueSource(string source, object value)
        {
            Source = source;

            if (value == null)
            {
                Value = "NULL";
            }
            else if (string.Empty.Equals(value))
            {
                Value = "EMPTY";
            }
            else
            {
                Value = value.ToString();
            }
        }

        public bool Equals(DiagnosticValueSource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Source, Source) && Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (DiagnosticValueSource)) return false;
            return Equals((DiagnosticValueSource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Source != null ? Source.GetHashCode() : 0)*397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Source: {0}, Value: {1}", Source, Value);
        }
    }
}