namespace FubuCore.Descriptions
{
    public interface IPrefixSource
    {
        string GetPrefix();
    }

    public class LiteralPrefixSource : IPrefixSource
    {
        private readonly string _text;

        public LiteralPrefixSource(int numberOfSpacesOnLeft, string text)
        {
            _text = "".PadRight(numberOfSpacesOnLeft) + text;
        }

        public string GetPrefix()
        {
            return _text;
        }
    }
}