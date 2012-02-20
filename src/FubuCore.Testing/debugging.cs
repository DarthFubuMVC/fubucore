using System;
using System.Diagnostics;
using System.Linq.Expressions;
using FubuCore.Testing.Reflection.Expressions;
using NUnit.Framework;

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


    }


}