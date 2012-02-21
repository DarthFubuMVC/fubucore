using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuCore.Util.TextWriting;

namespace FubuCore.Binding.Logging
{
    public class BindingReportTextWriter : IBindingReportVisitor
    {
        private readonly bool _showValues;
        private readonly Stack<BindingReport> _bindingStack = new Stack<BindingReport>();
        private readonly Stack<string> _descriptions = new Stack<string>();
        private readonly TextReport _report = new TextReport();

        public BindingReportTextWriter(BindingReport binding, bool showValues)
        {
            _showValues = showValues;

            addDivider();
            _report.AddText("Binding report for " + binding.ModelType.FullName);
            addDivider();

            if (showValues)
            {
                _report.StartColumns(3);
                _report.AddColumnData("Property", "Handler", "Values ('[RawValue]' from '[Source]'/[RawKey])");
            }
            else
            {
                _report.StartColumns(2);
                _report.AddColumnData("Property", "Handler");
            }

            addDivider();

            binding.AcceptVisitor(this);
            addDivider();
        }

        private void addDivider()
        {
            _report.AddDivider('=');
        }

        public TextReport Report
        {
            get { return _report; }
        }

        private void write(object handler, IEnumerable<BindingValue> values = null)
        {
            var description = Description.For(handler).Title;

            var propertyName = _descriptions.Reverse().Join("").Replace(".[", "[").TrimStart('.');
            if (_showValues)
            {
                var valueString = values == null
                                      ? string.Empty
                                      : values.Select(x => "'{0}' from '{1}'/{2}".ToFormat(x.RawValue, x.Source, x.RawKey)).Join(", ");
                _report.AddColumnData(propertyName, description, valueString);
            }
            else
            {
                _report.AddColumnData(propertyName, description);
            }
        
        }

        void IBindingReportVisitor.Report(BindingReport report)
        {
            _bindingStack.Push(report);
        }

        void IBindingReportVisitor.Property(PropertyBindingReport report)
        {
            _descriptions.Push("." + report.Property.Name);
            object handler = (object) report.Converter ?? report.Binder;
            write(handler, report.Values);
        }

        void IBindingReportVisitor.Element(ElementBinding binding)
        {
            _descriptions.Push("[{0}]".ToFormat(binding.Index));
            write(binding.Binder);
        }

        void IBindingReportVisitor.EndReport()
        {
            _bindingStack.Pop();
        }

        void IBindingReportVisitor.EndProperty()
        {
            _descriptions.Pop();
        }

        void IBindingReportVisitor.EndElement()
        {
            _descriptions.Pop();
        }
    }
}