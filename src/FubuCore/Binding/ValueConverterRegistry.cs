using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FubuCore.Binding
{
    public class ValueConverterRegistry : IValueConverterRegistry
    {
        private readonly List<IConverterFamily> _families = new List<IConverterFamily>();

        public ValueConverterRegistry(IEnumerable<IConverterFamily> families)
        {
            _families.AddRange(families);

            addPolicies();
        }

        public IEnumerable<IConverterFamily> Families
        {
            get { return _families; }
        }

        public ValueConverter FindConverter(PropertyInfo property)
        {
            var family = _families.FirstOrDefault(x => x.Matches(property));
            return family == null ? null : family.Build(this, property);
        }

        private void addPolicies()
        {
            // TODO -- move these declarations to FubuMVC
            Add<PassthroughConverter<HttpPostedFileBase>>();
            Add<PassthroughConverter<HttpFileCollectionWrapper>>();
            Add<PassthroughConverter<HttpCookie>>();

            Add<ExpandEnvironmentVariablesFamily>();
            Add<ResolveConnectionStringFamily>();

            Add<BooleanFamily>();
            Add<NumericTypeFamily>();
            Add<TypeDescriptorConverterFamily>();
        }

        public void Add<T>() where T : IConverterFamily, new()
        {
            _families.Add(new T());
        }
    }
}