namespace FubuCore.Testing.Csv
{
    public class TestCsvObject
    {
        protected bool Equals(TestCsvObject other)
        {
            return Count == other.Count && string.Equals(Name, other.Name) && Flag.Equals(other.Flag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Count;
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Flag.GetHashCode();
                return hashCode;
            }
        }

        public int Count { get; set; }
        public string Name { get; set; }
        public bool Flag { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestCsvObject) obj);
        }
    }
}