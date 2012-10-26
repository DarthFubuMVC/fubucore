namespace FubuCore.Descriptions
{
    public class UnorderedPrefixSource : IPrefixSource
    {
        private readonly string _prefix;

        public UnorderedPrefixSource(int numberOfSpaces)
        {
            _prefix = "".PadRight(numberOfSpaces, ' ');
        }

        public string GetPrefix()
        {
            return _prefix + "* ";
        }
    }
}