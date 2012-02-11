using System.Linq;
using FubuCore.Configuration;
using FubuTestingSupport;
using NUnit.Framework;
using SpecificationExtensions = FubuTestingSupport.SpecificationExtensions;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class SettingsProviderResolvableDiagnosticsTester
    {
        private SettingsData theSettings1;
        private SettingsData theSettings2;

        [SetUp]
        public void SetUp()
        {
            theSettings1 = new SettingsData()
                .With("Beer", "FreeState Wheat");

            theSettings1.Provenance = "host";

            theSettings2 = new SettingsData()
                .With("Snack", "{Beer} and Peanuts");
            theSettings2.Provenance = "other";
        }

        [Test]
        public void unresolved_report()
        {
            SettingsProvider.For(theSettings1, theSettings2).CreateDiagnosticReport()
                                                                     .Single(x => x.Key == "Snack").Value.ShouldEqual("{Beer} and Peanuts");
        }

        [Test]
        public void resolved_report()
        {
            SettingsProvider.For(theSettings1, theSettings2).CreateResolvedDiagnosticReport()
                                                                     .Single(x => x.Key == "Snack").Value.ShouldEqual("FreeState Wheat and Peanuts");
        }
    }
}