using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using FubuCore.Conversion;
using System.Linq;

namespace FubuCore.Binding
{
    [Description("Binds an array")]
    public class ArrayPropertyBinder : IPropertyBinder
    {
        private readonly ConverterLibrary _library;

        public ArrayPropertyBinder(ConverterLibrary library)
        {
            _library = library;
        }

        public bool Matches(PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            if (!propertyType.IsArray) return false;

            var elementType = propertyType.GetElementType();

            return !_library.CanBeParsed(elementType);
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            var builder = typeof (ArrayBuilder<>).CloseAndBuildAs<IArrayBuilder>(property.PropertyType.GetElementType());
            builder.FillValues(property, context);
        }

        public interface IArrayBuilder
        {
            void FillValues(PropertyInfo property, IBindingContext context);
        }

        public class ArrayBuilder<T> : IArrayBuilder
        {
            public void FillValues(PropertyInfo property, IBindingContext context)
            {
                var requests = context.RequestData.GetEnumerableRequests(property.Name).ToList();
                var data = new T[requests.Count];

                for (int i = 0; i < requests.Count; i++)
                {
                    var requestData = (IRequestData) requests[i];
                    data[i] = (T) context.BindObject(requestData, typeof (T)).Value;
                }

                property.SetValue(context.Object, data, null);
            }
        }
    }
}