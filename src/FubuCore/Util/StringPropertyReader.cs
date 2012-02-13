using System;

namespace FubuCore.Util
{
    public class StringPropertyReader
    {
        private readonly string _text;

        public StringPropertyReader(string text)
        {
            _text = text;
        }

        public void ReadProperties(Action<string, string> callback)
        {
            var lastLine = "";

            _text.ReadLines(line =>
            {
                if (line.IsEmpty()) return;

                if (line.StartsWith(" ") && lastLine.IsNotEmpty())
                {
                    var trimmed = line.TrimStart();
                    var spaces = line.Length - trimmed.Length;

                    line = lastLine.Substring(0, spaces) + trimmed;
                }

                lastLine = line;

                var parts = line.Split('=');
                callback(parts[0], parts[1]);
            });
        }

        public Cache<string, string> ReadProperties()
        {
            var cache = new Cache<string, string>();
            ReadProperties((key, value) =>
            {
                cache[key] = value;
            });

            return cache;
        }
    }
}