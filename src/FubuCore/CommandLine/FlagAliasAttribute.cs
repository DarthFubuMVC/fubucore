using System;

namespace FubuCore.CommandLine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FlagAliasAttribute : Attribute
    {
        private readonly string _alias;
        private readonly char _oneLetterAlias;

        [Obsolete("Must provide long (--flag) and short (-f) form when aliasing a flag. Use other FlagAliasAttribute constructor")]
        public FlagAliasAttribute(string alias)
            : this(alias, alias[0])
        {
        }

        public FlagAliasAttribute(string alias, char oneLetterAlias)
        {
            _alias = alias;
            _oneLetterAlias = oneLetterAlias;
        }

        public string Alias
        {
            get { return _alias; }
        }

        public char OneLetterAlias
        {
            get { return _oneLetterAlias; }
        }
    }
}