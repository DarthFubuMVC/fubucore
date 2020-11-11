using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace FubuCore.Binding
{
    [Description("Binds an array")]
    public class ArrayPropertyBinder : IPropertyBinder
    {
        private readonly ConversionPropertyBinder _conversionPropertyBinder;

        public ArrayPropertyBinder(ConversionPropertyBinder conversionPropertyBinder)
        {
            _conversionPropertyBinder = conversionPropertyBinder;
        }

        public bool Matches(PropertyInfo property)
        {
            return property.PropertyType.IsArray;
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            var builder = typeof (ArrayBuilder<>).CloseAndBuildAs<IArrayBuilder>(_conversionPropertyBinder ,property.PropertyType.GetElementType());
            builder.FillValues(property, context);
        }

        public interface IArrayBuilder
        {
            void FillValues(PropertyInfo property, IBindingContext context);
        }

        public class ArrayBuilder<T> : IArrayBuilder
        {
            private readonly ConversionPropertyBinder _conversionPropertyBinder;

            public ArrayBuilder(ConversionPropertyBinder conversionPropertyBinder)
            {
                _conversionPropertyBinder = conversionPropertyBinder;
            }

            public void FillValues(PropertyInfo property, IBindingContext context)
            {
                if (_conversionPropertyBinder.CanBeParsed(property.PropertyType))
                {
                    bool convertedAsIs = context.Data.ValueAs<string>(property.Name, value => _conversionPropertyBinder.Bind(property, context));
                    if (convertedAsIs) return;
                }

                var requests = context.GetEnumerableRequests(property.Name).ToList();

                // TODO -- need an end to end test on this behavior 
                if (!requests.Any()) return;

                var data = new T[requests.Count];

                for (int i = 0; i < requests.Count; i++)
                {
                    var requestData = requests[i];

                    context.Logger.PushElement(typeof(T));

                    // TODO -- got to add the BindResult to context to store it later
                    context.BindObject(requestData, typeof (T), o =>
                    {
                        data[i] = (T) o;
                    });
                }

                property.SetValue(context.Object, data, null);
            }
        }
    }
}