using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Testing.Reflection.Expressions;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing
{
    [TestFixture, Explicit]
    public class debugging
    {
        [Test]
        public void look_at_it()
        {
            Expression<Func<string, Component>> ctor = x => new Component(x);

            Debug.WriteLine(ctor);
        }

        [Test]
        public void understanding_writeline()
        {
            Console.WriteLine("***********************************");
            Console.WriteLine();
            Console.Write("a");
            Console.Write("a");
            Console.Write("a");
            Console.WriteLine("***********************************");
        }

        [Test]
        public void what_closes()
        {
            ReflectionHelper.GetProperty<EnumClass>(x => x.List).PropertyType.Closes(typeof(IEnumerable<>)).ShouldBeTrue();
        }

        public class EnumClass
        {
            public IList<Target> List { get; set; }
            public ICollection<Target> Collection { get; set; }
            public IEnumerable<Target> Enumerable { get; set; }
        }

        public class Target{}

    }


}