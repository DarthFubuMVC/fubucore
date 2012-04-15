using System;

namespace FubuCore.CommandLine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FlagAliasAttribute : Attribute
    {
        private readonly string _longAlias;
        private readonly char _oneLetterAlias;

        public FlagAliasAttribute(string longAlias, char oneLetterAlias)
        {
            _longAlias = longAlias;
            _oneLetterAlias = oneLetterAlias;
        }

        public string LongAlias
        {
            get { return _longAlias; }
        }

        public char OneLetterAlias
        {
            get { return _oneLetterAlias; }
        }
    }
}