using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FubuCore.Binding.Logging;

namespace FubuCore.Binding.InMemory
{
    public interface IBindingHistory
    {
        void Store(BindingReport report);
    }

    public class InMemoryBindingHistory : IBindingHistory
    {
        private readonly IList<BindingReport> _reports = new List<BindingReport>();

        public void Store(BindingReport report)
        {
            _reports.Add(report);
        }

        public IEnumerable<BindingReport> AllReports
        {
            get
            {
                return _reports;
            }
        }
    }

    public class RecordingBindingLogger : IBindingLogger
    {
        private readonly IBindingHistory _history;
        private readonly Stack<BindingReport> _models = new Stack<BindingReport>();
        private Type _nextElement;

        public RecordingBindingLogger(IBindingHistory history)
        {
            _history = history;
        }

        private BindingReport currentReport
        {
            get { return _models.Peek(); }
        }

        public void Chose(Type modelType, IModelBinder binder)
        {
            var report = startReport(modelType, binder);
            _nextElement = null;
            _models.Push(report);
        }

        // This acts as a de facto PushProperty
        public void Chose(PropertyInfo property, IPropertyBinder binder)
        {
            // Don't do anything if this has happened in the middle of a set properties operation
            if (_models.Any())
            {
                currentReport.AddProperty(property, binder);
            }
            
        }

        public void Chose(PropertyInfo property, ValueConverter converter)
        {
            if (_models.Any())
            {
                currentReport.For(property).Chose(converter);
            }
        }

        public void PushElement(Type elementType)
        {
            _nextElement = elementType;
        }

        public void FinishedModel()
        {
            if (_models.Count == 1)
            {
                _history.Store(currentReport);
            }

            _models.Pop();
        }

        public void UsedValue(BindingValue value)
        {
            if (_models.Any())
            {
                currentReport.Used(value);
            }
        }

        private BindingReport startReport(Type modelType, IModelBinder binder)
        {
            if (_nextElement != null && _models.Any())
            {
                return currentReport.LastProperty.AddElement(_nextElement, binder);
            }

            if (_models.Any())
            {
                return currentReport.LastProperty.BindAsNestedChild(binder);
            }

            return new BindingReport(modelType, binder);
        }
    }
}