using System.IO;

namespace FubuCore.Util.TextWriting
{
    internal interface Line
    {
        void OverwriteCounts(CharacterWidth[] widths);
        void Write(TextWriter writer, CharacterWidth[] widths);
    }
}