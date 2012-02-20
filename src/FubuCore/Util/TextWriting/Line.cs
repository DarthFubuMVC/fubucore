using System.IO;

namespace FubuCore.Util.TextWriting
{
    public interface Line
    {
        void WriteToConsole();
        void Write(TextWriter writer);
        int Width { get; }
    }
}