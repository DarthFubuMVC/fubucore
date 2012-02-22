using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FubuCore.Binding
{
    [Description("Binds a collection or list property")]
    public class CollectionPropertyBinder : IPropertyBinder
    {
        public bool Matches(PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            if (propertyType == typeof (string)) return false;

            return propertyType.Closes(typeof (IEnumerable<>));
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            var type = property.PropertyType;
            var elementType = type.FindInterfaceThatCloses(typeof (IEnumerable<>)).GetGenericArguments().Single();

            var builder = typeof (EnumerableBuilder<>).CloseAndBuildAs<IEnumerableBuilder>(elementType);
            builder.FillValues(property, context);
        }

        #region Nested type: EnumerableBuilder

        public class EnumerableBuilder<T> : IEnumerableBuilder
        {
            public void FillValues(PropertyInfo property, IBindingContext context)
            {
                var collection = property.GetValue(context.Object, null) as ICollection<T>;
                if (collection == null)
                {
                    collection = new List<T>();
                    property.SetValue(context.Object, collection, null);
                }

                context.GetEnumerableRequests(property.Name).Each(request =>
                {
                    context.Logger.PushElement(typeof (T));

                    // TODO -- got to add the BindResult to context to store it later
                    context.BindObject(request, typeof (T), @object => { collection.Add((T) @object); });
                });
            }
        }

        #endregion

        #region Nested type: IEnumerableBuilder

        public interface IEnumerableBuilder
        {
            void FillValues(PropertyInfo property, IBindingContext context);
        }

        #endregion
    }
}