using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;

namespace FubuCore.Binding.Logging
{
    public class AccessorLogger : IBindingLogger
    {
        private readonly IBindingLogger _realLogger;
        private readonly Stack<AccessorReport> _models = new Stack<AccessorReport>();
        private Type _nextElement;

        public AccessorLogger()
            : this(new NulloBindingLogger())
        {
        }

        public AccessorLogger(IBindingLogger realLogger)
        {
            _realLogger = realLogger;
        }

        void IBindingLogger.Chose(Type modelType, IModelBinder binder)
        {
            var report = startReport();
            _nextElement = null;
            _models.Push(report);

            _realLogger.Chose(modelType, binder);
        }

        void IBindingLogger.Chose(PropertyInfo property, IPropertyBinder binder)
        {
            if (_models.Any())
            {
                currentReport.AddNode(property);
            }

            _realLogger.Chose(property, binder);
        }

        void IBindingLogger.Chose(PropertyInfo property, ValueConverter converter)
        {
            _realLogger.Chose(property, converter);
        }

        void IBindingLogger.PushElement(Type elementType)
        {
            _nextElement = elementType;

            _realLogger.PushElement(elementType);
        }

        void IBindingLogger.FinishedModel()
        {
            _models.Pop();

            _realLogger.FinishedModel();
        }

        void IBindingLogger.UsedValue(BindingValue value)
        {
            _realLogger.UsedValue(value);
        }

        private AccessorReport currentReport
        {
            get { return _models.Peek(); }
        }


        private AccessorReport startReport()
        {
            if (_nextElement != null && _models.Any())
            {
                return currentReport.LastNode.Element();
            }

            if (_models.Any())
            {
                return currentReport.LastNode.Nested();
            }

            return new AccessorReport();
        }

        public Accessor AccessorOf(PropertyInfo property)
        {
            if (property == null || _models.Count == 0)
            {
                return null;
            }
            return currentReport.NodeOf(property).Accessor;
        }
    }
}