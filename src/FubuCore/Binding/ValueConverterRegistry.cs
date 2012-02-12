using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Conversion;

namespace FubuCore.Binding
{
    [MarkedForTermination("Replacing w/ the BindingRegistry")]
    public class ValueConverterRegistry : IValueConverterRegistry
    {
        private readonly IEnumerable<IConverterFamily> _families;
        private readonly List<IConverterFamily> _defaultFamilies = new List<IConverterFamily>{
                new ExpandEnvironmentVariablesFamily(),
                new ResolveConnectionStringFamily(),
                new BooleanFamily(),
                new NumericTypeFamily(),
                new ObjectTypeFamily()
        };

        public ValueConverterRegistry(IEnumerable<IConverterFamily> families, ConverterLibrary library)
        {
            if (library == null) throw new ArgumentNullException("library");

            _families = families;

            // TODO -- gotta get rid of this
            _defaultFamilies.Add(new BasicConverterFamily(library));
        }

        public IEnumerable<IConverterFamily> Families
        {
            get
            {
                foreach (var family in _families)
                {
                    yield return family;
                }

                foreach (var family in _defaultFamilies)
                {
                    yield return family;
                }
            }
        }

        public ValueConverter FindConverter(PropertyInfo property)
        {
            var family = Families.FirstOrDefault(x => x.Matches(property));
            return family == null ? null : family.Build(this, property);
        }
    }
}