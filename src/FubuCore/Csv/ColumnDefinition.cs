using FubuCore.Reflection;

namespace FubuCore.Csv
{
    public class ColumnDefinition
    {
        private readonly Accessor _accessor;

        public ColumnDefinition(Accessor accessor)
        {
            _accessor = accessor;
        }

        public Accessor Accessor
        {
            get { return _accessor; }
        }

        public string Alias { get; set; }

        public string Name
        {
            get
            {
                if (Alias.IsNotEmpty()) return Alias;
                return Accessor.Name;
            }
        }

        public override string ToString()
        {
            return "Column {0}:{1}".ToFormat(Name, Accessor.Name);
        }
    }
}