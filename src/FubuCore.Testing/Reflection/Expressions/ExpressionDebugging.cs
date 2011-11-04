using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Reflection.Expressions;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Expressions
{
    

    [TestFixture, Explicit]
    public class ExpressionDebugging
    {
        [Test]
        public void blah()
        {
            Expression<Func<Kase, object>> foo = x => x.Queue;
            var bb = new CollectionContainsPropertyOperation();
            var aa = bb.GetPredicateBuilder<Kase>(foo.GetMemberExpression(true));
            var caseToTest = new Kase{Queue = new Kueue{Name = "foo"}};
            var listOfQueues = new List<Kueue> {new Kueue {Name = "foo"}, new Kueue {Name = "bar"}};
            aa(listOfQueues).Compile()(caseToTest).ShouldBeTrue();
        }

        //test aa        
        private class Kueue
        {
            public string Name { get; set; }

            public bool Equals(Kueue other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Name, Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (Kueue)) return false;
                return Equals((Kueue) obj);
            }

            public override int GetHashCode()
            {
                return (Name != null ? Name.GetHashCode() : 0);
            }
        }

        private class Kase
        {
            public Kueue Queue { get; set; }
        }
    }
}