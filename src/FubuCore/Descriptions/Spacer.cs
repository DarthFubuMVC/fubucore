namespace FubuCore.Descriptions
{
    public class Spacer : IPrefixSource
    {
        private readonly string _prefix;

        public Spacer(int numberOfSpaces)
        {
            _prefix = "".PadRight(numberOfSpaces, ' ');
        }

        public string GetPrefix()
        {
            return _prefix;
        }
    }
}