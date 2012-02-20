using System;
using System.Collections.Generic;
using System.IO;
using FubuCore.Util.TextWriting;
using System.Linq;

namespace FubuCore.Descriptions
{
    public interface IPrefixSource
    {
        string GetPrefix();
    }

    public class Spacer : IPrefixSource
    {
        private readonly string _prefix;

        public Spacer(int numberOfSpaces)
        {
            _prefix = "".PadRight(numberOfSpaces, ' ');
        }

        public string GetPrefix()
        {
            return _prefix;
        }
    }

    public class NumberedPrefixSource : IPrefixSource
    {
        private readonly string _prefix;
        private int _number;

        public NumberedPrefixSource(int numberOfSpaces)
        {
            _prefix = "".PadRight(numberOfSpaces, ' ');
            _number = 0;
        }

        public string GetPrefix()
        {
            return _prefix + (++_number).ToString().PadLeft(3) + ".) ";
        }
    }

    public class UnorderedPrefixSource : IPrefixSource
    {
        private readonly string _prefix;

        public UnorderedPrefixSource(int numberOfSpaces)
        {
            _prefix = "".PadRight(numberOfSpaces, ' ');
        }

        public string GetPrefix()
        {
            return _prefix + "* ";
        }
    }

    public class DescriptionTextWriter : IDescriptionVisitor
    {
        private readonly int TabSpaces = 4;
        private readonly TextReport _report = new TextReport();
        private int _level = 0;
        private readonly Stack<IPrefixSource> _prefixes = new Stack<IPrefixSource>();


        public DescriptionTextWriter(Description description)
        {
            if (!description.IsMultiLevel())
            {
                _report.AddText(description.ToString());
            }
            else
            {
                description.AcceptVisitor(this);
            }
            
        }

        public void WriteToConsole()
        {
            _report.WriteToConsole();
        }

        public override string ToString()
        {
            var writer = new StringWriter();
            _report.Write(writer);

            return writer.ToString();
        }

        void IDescriptionVisitor.Start(Description description)
        {
            if (_level == 0)
            {
                _report.AddDivider('=');
                _report.AddText(description.ToString());
                _report.AddDivider('=');
            }
            else
            {
                _report.AddColumnData(_prefixes.Peek().GetPrefix() + description.Title, description.ShortDescription);    
            }
        }

        private int numberOfSpacesOnLeft
        {
            get
            {
                return _level*TabSpaces;
            }
        }

        private string spacer()
        {
            return "".PadRight(numberOfSpacesOnLeft, ' ');
        }

        void IDescriptionVisitor.StartList(BulletList list)
        {
            _level++;
            _report.AddText(spacer() + " ** " + (list.Label ?? list.Name));
            _level++;

            if (list.IsOrderDependent)
            {
                _prefixes.Push(new NumberedPrefixSource(numberOfSpacesOnLeft));
            }
            else
            {
                _prefixes.Push(new UnorderedPrefixSource(numberOfSpacesOnLeft));
            }

            _report.StartColumns(2);
        }

        void IDescriptionVisitor.EndList()
        {
            _prefixes.Pop();
            _level--;
            _level--;
            _report.EndColumns();
        }

        void IDescriptionVisitor.End()
        {
            
        }
    }
}