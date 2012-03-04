using FubuCore.Binding.Values;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuCore.Testing.Binding.Values
{
    [TestFixture]
    public class ValueDiagnosticReportTester
    {

        [Test]
        public void write_first_level_properties()
        {
            var report = new ValueDiagnosticReport();
            
            report.Value("C", 3);
            report.Value("A", 1);
            report.Value("B", 2);
            

            report.AllValues().Select(x => x.Key)
                .ShouldHaveTheSameElementsAs("A", "B", "C");
        }

        [Test]
        public void writes_the_source()
        {
            var report = new ValueDiagnosticReport();

            report.StartSource(new DictionaryValueSource(null, "Ralph"));
            report.Value("A", 1);
            report.EndSource();


            report.StartSource(new DictionaryValueSource(null, "Potsie"));
            report.Value("A", 2);
            report.EndSource();


            report.AllValues().Single().Key.ShouldEqual("A");

            var value = report.For("A");
            value.First().ShouldEqual(new DiagnosticValueSource("Ralph", 1));

            value.ShouldHaveTheSameElementsAs(new DiagnosticValueSource("Ralph", 1), new DiagnosticValueSource("Potsie", 2));
            
        }

        [Test]
        public void write_second_level_properties()
        {
            var report = new ValueDiagnosticReport();
            report.StartChild("A");
            report.Value("B", 2);

            report.StartChild("C");
            report.Value("D", 4);

            report.EndChild();
            report.Value("E", 5);

            report.EndChild();
            report.Value("F", 6);

            report.AllValues().Select(x => x.Key)
                .ShouldHaveTheSameElementsAs("A.B", "A.C.D", "A.E", "F");
        }

        [Test]
        public void write_enumerable_properties()
        {
            var report = new ValueDiagnosticReport();
            report.StartChild("A", 0);
            report.Value("B", 0);
            report.EndChild();

            report.StartChild("A", 1);
            report.Value("B", 1);
            report.EndChild();

            report.StartChild("A", 2);
            report.Value("B", 2);
            report.EndChild();

            report.AllValues().Select(x => x.Key)
                .ShouldHaveTheSameElementsAs("A[0].B", "A[1].B", "A[2].B");
            
        }

        [Test]
        public void create_DiagnosticValueSource_with_a_null()
        {
            var source = new DiagnosticValueSource("something", null);
            source.Value.ShouldEqual("NULL");
        }

        [Test]
        public void create_DiagnosticValueSource_with_non_null_non_empty()
        {
            var source = new DiagnosticValueSource("something", 1);
            source.Value.ShouldEqual("1");
        }

        [Test]
        public void create_DiagnosticValueSource_with_empty_string()
        {
            var source = new DiagnosticValueSource("something", string.Empty);
            source.Value.ShouldEqual("EMPTY");
        }
    }
}