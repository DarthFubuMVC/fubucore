using System.Collections.Generic;
using FubuCore.Binding.Values;
using FubuCore.Configuration;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing.Binding.Values
{
    [TestFixture]
    public class DictionaryValueSourceReportWritingTester
    {
        private SettingsData theSource;
        private Mock<IValueReport> theReport;

        [SetUp]
        public void SetUp()
        {
            theSource = new SettingsData(new Dictionary<string, object>(), "Something");
            theReport = new Mock<IValueReport>(MockBehavior.Strict);
        }

        [Test]
        public void simple_value()
        {
            theReport = new Mock<IValueReport>();

            theSource.Set("A", 1);

            theSource.WriteReport(theReport.Object);
        
            theReport.Verify(x => x.Value("A", 1));
        }

        [Test]
        public void nested_child()
        {
            theSource.Child("Child").As<SettingsData>().Set("A", 1);

            var sequence = new MockSequence();
            theReport.InSequence(sequence).Setup(x => x.StartChild("Child"));
            theReport.InSequence(sequence).Setup(x => x.Value("A", 1));
            theReport.InSequence(sequence).Setup(x => x.EndChild());

            theSource.WriteReport(theReport.Object);

            theReport.VerifyAll();
        }

        [Test]
        public void children()
        {
            theSource.GetChildrenElement("Children", 0).Set("A", 0);
            theSource.GetChildrenElement("Children", 1).Set("A", 1);
            theSource.GetChildrenElement("Children", 2).Set("A", 2);

            var sequence = new MockSequence();
            theReport.InSequence(sequence).Setup(x => x.StartChild("Children", 0));
            theReport.InSequence(sequence).Setup(x => x.Value("A", 0));
            theReport.InSequence(sequence).Setup(x => x.EndChild());

            theReport.InSequence(sequence).Setup(x => x.StartChild("Children", 1));
            theReport.InSequence(sequence).Setup(x => x.Value("A", 1));
            theReport.InSequence(sequence).Setup(x => x.EndChild());

            theReport.InSequence(sequence).Setup(x => x.StartChild("Children", 2));
            theReport.InSequence(sequence).Setup(x => x.Value("A", 2));
            theReport.InSequence(sequence).Setup(x => x.EndChild());

            theSource.WriteReport(theReport.Object);
            theReport.VerifyAll();
        }
    }
}