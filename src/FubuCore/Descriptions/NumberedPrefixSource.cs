namespace FubuCore.Descriptions
{
    public class NumberedPrefixSource : IPrefixSource
    {
        private readonly string _prefix;
        private int _number;

        public NumberedPrefixSource(int numberOfSpaces)
        {
            _prefix = "".PadRight(numberOfSpaces, ' ');
            _number = 0;
        }

        public string GetPrefix()
        {
            return _prefix + (++_number).ToString().PadLeft(3) + ".) ";
        }
    }
}