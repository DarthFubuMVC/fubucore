using System.Collections.Generic;
using FubuCore.Binding.Values;
using FubuCore.Configuration;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding.Values
{
    [TestFixture]
    public class DictionaryValueSourceReportWritingTester
    {
        private SettingsData theSource;
        private IValueReport theReport;
        private MockRepository theMocks;

        [SetUp]
        public void SetUp()
        {
            theMocks = new MockRepository();
            theSource = new SettingsData(new Dictionary<string, object>(), "Something");
            theReport = theMocks.StrictMock<IValueReport>();
        }

        [Test]
        public void simple_value()
        {
            theReport = MockRepository.GenerateMock<IValueReport>();

            theSource.Set("A", 1);

            theSource.WriteReport(theReport);
        
            theReport.AssertWasCalled(x => x.Value("A", 1));
        }

        [Test]
        public void nested_child()
        {
            theSource.Child("Child").As<SettingsData>().Set("A", 1);

            using (theMocks.Ordered())
            {
                theReport.StartChild("Child");

                theReport.Value("A", 1);

                theReport.EndChild();
            }

            theMocks.ReplayAll();

            theSource.WriteReport(theReport);

            theMocks.VerifyAll();
        }

        [Test]
        public void children()
        {
            theSource.GetChildrenElement("Children", 0).Set("A", 0);
            theSource.GetChildrenElement("Children", 1).Set("A", 1);
            theSource.GetChildrenElement("Children", 2).Set("A", 2);
        
            using (theMocks.Ordered())
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


            }
        }
    }
}