using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Csv
{
    public class CsvTokenizer
    {
        readonly List<string> _tokens;
        readonly List<char> _characters;
        Mode _mode;
        readonly Dictionary<Mode, Action<char>> _read;
        const char Comma = ',';
        const char DoubleQuote = '"';

        public CsvTokenizer()
        {
            _tokens = new List<string>();
            _characters = new List<char>();
            _read = new Dictionary<Mode, Action<char>>
            {
                { Mode.Normal, normalRead },
                { Mode.Escape, escapeRead },
                { Mode.ExitingEscape, exitingEscapeRead }
            };
        }

        public IEnumerable<string> Tokens
        {
            get { return _tokens; }
        }

        public bool IsPendingForMoreLines
        {
            get { return _mode == Mode.Escape; }
        }

        public void Read(string line)
        {
            if (IsPendingForMoreLines)
            {
                addChar('\n');
            }
            line.Each(read);
            if (!IsPendingForMoreLines)
            {
                MarkReadComplete();
            }
        }

        public void MarkReadComplete()
        {
            _mode = Mode.Normal;
            if (!_characters.Any()) return;
            tokenize();
        }

        public void Reset()
        {
            _tokens.Clear();
            _characters.Clear();
            _mode = Mode.Normal;
        }

        private void read(char c)
        {
            _read[_mode](c);
        }

        private void tokenize()
        {
            _tokens.Add(new string(_characters.ToArray()));
            _characters.Clear();
        }

        private void addChar(char c)
        {
            _characters.Add(c);
        }

        private void mode(Mode mode)
        {
            _mode = mode;
        }

        private void normalRead(char c)
        {
            switch (c)
            {
                case Comma:
                    tokenize();
                    break;
                case DoubleQuote:
                    mode(Mode.Escape);
                    break;
                default:
                    addChar(c);
                    break;
            }
        }

        private void escapeRead(char c)
        {
            if (c == DoubleQuote)
            {
                mode(Mode.ExitingEscape);
            }
            else
            {
                addChar(c);
            }
        }

        private void exitingEscapeRead(char c)
        {
            mode(Mode.Normal);
            switch (c)
            {
                case Comma:
                    tokenize();
                    break;
                case DoubleQuote:
                    mode(Mode.Escape);
                    addChar(c);
                    break;
                default:
                    addChar(c);
                    break;
            }
        }

        private enum Mode
        {
            Normal,
            Escape,
            ExitingEscape
        }
    }
}