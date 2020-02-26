using System.Collections.Generic;
using FubuCore.Binding.Values;
using FubuCore.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace FubuCore.Testing.Binding.Values
{
    [TestFixture]
    public class DictionaryValueSourceReportWritingTester
    {
        private SettingsData theSource;
        private IValueReport theReport;

        [SetUp]
        public void SetUp()
        {
            theSource = new SettingsData(new Dictionary<string, object>(), "Something");
            theReport = Substitute.For<IValueReport>();
        }

        [Test]
        public void simple_value()
        {
            theReport = Substitute.For<IValueReport>();

            theSource.Set("A", 1);

            theSource.WriteReport(theReport);
        
            theReport.Received().Value("A", 1);
        }

        [Test]
        public void nested_child()
        {
            theSource.Child("Child").As<SettingsData>().Set("A", 1);

            theSource.WriteReport(theReport);

            Received.InOrder(() =>
            {
                theReport.StartChild("Child");

                theReport.Value("A", 1);

                theReport.EndChild();
            });
        }

        [Test]
        public void children()
        {
            theSource.GetChildrenElement("Children", 0).Set("A", 0);
            theSource.GetChildrenElement("Children", 1).Set("A", 1);
            theSource.GetChildrenElement("Children", 2).Set("A", 2);
            theSource.WriteReport(theReport);
        
            Received.InOrder(() =>
            {
                theReport.StartChild("Children", 0);
                theReport.Value("A", 0);
                theReport.EndChild();

                theReport.StartChild("Children", 1);
                theReport.Value("A", 1);
                theReport.EndChild();

                theReport.StartChild("Children", 2);
                theReport.Value("A", 2);
                theReport.EndChild();
            });
        }
    }
}