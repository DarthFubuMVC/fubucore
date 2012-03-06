using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Util
{
    public class StringPropertyReader
    {
        private readonly Action<Action<string>> _reader;

        private StringPropertyReader(Action<Action<string>> reader)
        {
            _reader = reader;
        }

        public static StringPropertyReader ForText(string text)
        {
            return new StringPropertyReader(text.ReadLines);
        }

        public static StringPropertyReader ForFile(string file)
        {
            var fileSystem = new FileSystem();

            return new StringPropertyReader(callback => fileSystem.ReadTextFile(file, callback));
        }

        public void ReadProperties(Action<string, string> callback)
        {
            var lastLine = "";

            _reader.Invoke(line =>
            {
                if (line.IsEmpty()) return;

                if (line.StartsWith(" ") && lastLine.IsNotEmpty())
                {
                    var trimmed = line.TrimStart();
                    var spaces = line.Length - trimmed.Length;

                    line = lastLine.Substring(0, spaces) + trimmed;
                }

                lastLine = line;


                // This code below becomes the ValueSource.Read(text) method
                ReadLine(line, callback);
            });
        }

        public static void ReadLine(string text, Action<string, string> callback)
        {
            var parts = text.Split('=');
            if (parts.Length <= 1)
            {
                throw new Exception("Invalid settings data text for '{0}'".ToFormat(text));
            }

            var key = parts[0].Trim();
            var value = parts.Skip(1).Join("=").Trim();

            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
            }

            callback(key, value);
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