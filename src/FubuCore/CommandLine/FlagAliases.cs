namespace FubuCore.CommandLine
{
    public class FlagAliases
    {
        public string LongForm { get; set; }
        public string ShortForm { get; set; }

        public bool Matches(string token)
        {
            var lowerToken = token.ToLower();

            return lowerToken == ShortForm.ToLower() || lowerToken == LongForm.ToLower();
        }

        public override string ToString()
        {
            return "{0}, {1}".ToFormat(ShortForm, LongForm);
        }
    }
}