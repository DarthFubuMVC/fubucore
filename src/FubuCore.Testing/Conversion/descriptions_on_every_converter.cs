using System;
using System.Collections.Generic;
using System.Diagnostics;
using FubuCore.Conversion;
using FubuCore.Descriptions;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuCore.Testing.Conversion
{
    [TestFixture]
    public class descriptions_on_every_converter
    {
        

        [Test]
        public void must_be_some_sort_of_description_on_every_iconverter_family()
        {
            var converter = new ConverterLibrary();
            Debug.WriteLine(converter.WhatDoIHave());
            Debug.WriteLine("");
            Debug.WriteLine("");
            Debug.WriteLine("");

            var types = typeof (IObjectConverterFamily).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IObjectConverterFamily>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Debug.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        
            
        }
    }
}