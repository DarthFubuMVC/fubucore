namespace FubuCore.Descriptions
{
    public interface IPrefixSource
    {
        string GetPrefix();
    }

    public class LiteralPrefixSource : IPrefixSource
    {
        private readonly string _text;

        public LiteralPrefixSource(string text)
        {
            _text = text;
        }

        public string GetPrefix()
        {
            return _text;
        }
    }
}