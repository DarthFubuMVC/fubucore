using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FubuCore.Util.TextWriting;
using System.Linq;

namespace FubuCore.Descriptions
{
    public static class DescriptionExtensions
    {
        public static string ToDescriptionText(this object target)
        {
            var description = Description.For(target);
            var writer = new DescriptionTextWriter(description);

            return writer.ToString();
        }

        public static void WriteDescriptionToConsole(this object target)
        {
            var description = Description.For(target);
            var writer = new DescriptionTextWriter(description);

            writer.WriteToConsole();
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
            if (description.IsMultiLevel())
            {
                description.AcceptVisitor(this);
            }
            else
            {
                this.As<IDescriptionVisitor>().Start(description);
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

                writeProperties(4, description);

                writeChildren(4, description);
            }
            else
            {
                var prefix = _prefixes.Peek().GetPrefix();
                var indent = prefix.Length;
                if (_childName.IsNotEmpty())
                {
                    prefix = prefix + _childName + ":";
                    indent += 5;
                }

                var firstColumn = _childName.IsEmpty() ? prefix : prefix + _childName + ":";
                _report.AddColumnData(firstColumn, description.Title, description.ShortDescription ?? string.Empty);

                

                writeProperties(indent, description);
                writeChildren(indent, description);
            }
        }

        private void writeChildren(int indent, Description description)
        {
            if (!description.Children.Any()) return;

            if (description.Properties.Any())
            {
                _report.AddText("");
            }

            _level++;

            _report.StartColumns(new Column(ColumnJustification.left, indent, 5), new Column(ColumnJustification.left, 0, 5), new Column(ColumnJustification.left, 0, 0));
            
                        
            description.Children.Each((name, child) => {

                _prefixes.Push(new LiteralPrefixSource(numberOfSpacesOnLeft, " * "));
                _childName = name;

                child.AcceptVisitor(this);

                _childName = null;
                _prefixes.Pop();

            });

            _level--;
            _report.EndColumns();



        }

        private void writeProperties(int indent, Description description)
        {
            _report.StartColumns(2);
            
            var spaces = "".PadRight(indent, ' ') + " * ";

            description.Properties.Each((key, prop) =>
            { if (prop != null) _report.AddColumnData(spaces + key, prop.ToString()); });

            _report.EndColumns();
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

        private readonly string _icon = " ** ";
        private string _childName;

        void IDescriptionVisitor.StartList(BulletList list)
        {
            _level++;
            _report.AddText(spacer() + _icon + (list.Label ?? list.Name));
            _level++;

            if (list.IsOrderDependent)
            {
                _prefixes.Push(new NumberedPrefixSource(numberOfSpacesOnLeft));
            }
            else
            {
                addUnorderedPrefix();
            }

            _report.StartColumns(new Column(ColumnJustification.right, 0, 0), new Column(ColumnJustification.left, 0, 5), new Column(ColumnJustification.left, 0, 0));
        }

        private void addUnorderedPrefix()
        {
            _prefixes.Push(new UnorderedPrefixSource(numberOfSpacesOnLeft));
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