using System.Globalization;
using System.Threading;
using FubuCore;
using FubuLocalization.Basic;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuLocalization.Tests.Basic
{
    [TestFixture]
    public class LocalizationProviderFactoryIntegratedTester
    {
        [SetUp]
        public void SetUp()
        {
            var system = new FileSystem();
            system.DeleteDirectory("localization1");
            system.DeleteDirectory("localization2");
            system.DeleteDirectory("localization3");

            system.CreateDirectory("localization1");
            system.CreateDirectory("localization2");
            system.CreateDirectory("localization3");
        }

        // values = "value=display"
        private void write(string directory, CultureInfo culture, string values)
        {
            XmlDirectoryLocalizationStorage.Write(directory, culture, LocalString.ReadAllFrom(values));
        }

        [Test]
        public void load_all()
        {
            write("localization1", new CultureInfo("en-US"),
                  @"
                a=us-a
                b=us-b
                f=us-f
            ");

            write("localization2", new CultureInfo("en-US"),
                  @"
                c=us-c
                d=us-d
            ");

            write("localization3", new CultureInfo("en-US"), @"
                e=us-e
            ");

            write("localization1", new CultureInfo("en-GB"),
                  @"
                a=gb-a
                b=gb-b
                f=gb-f
            ");

            write("localization2", new CultureInfo("en-GB"),
                  @"
                c=gb-c
                d=gb-d
            ");


            var source = new XmlDirectoryLocalizationStorage(new[]{"localization1", "localization2", "localization3"});
            var factory = new LocalizationProviderFactory(source, null, new LocalizationCache());
            factory.LoadAll(x => { });

            factory.BuildProvider(new CultureInfo("en-US"))
                .GetTextForKey(StringToken.FromKeyString("a"))
                .ShouldEqual("us-a");

            factory.BuildProvider(new CultureInfo("en-US"))
                .GetTextForKey(StringToken.FromKeyString("e"))
                .ShouldEqual("us-e");

            factory.BuildProvider(new CultureInfo("en-GB"))
                .GetTextForKey(StringToken.FromKeyString("a"))
                .ShouldEqual("gb-a");
        }

        [Test]
        public void put_it_all_together()
        {
            write("localization1", new CultureInfo("en-US"),
                  @"
                a=us-a
                b=us-b
                f=us-f
            ");

            write("localization2", new CultureInfo("en-US"),
                  @"
                c=us-c
                d=us-d
            ");

            write("localization3", new CultureInfo("en-US"), @"
                e=us-e
            ");

            write("localization1", new CultureInfo("en-GB"),
                  @"
                a=gb-a
                b=gb-b
                f=gb-f
            ");

            write("localization2", new CultureInfo("en-GB"),
                  @"
                c=gb-c
                d=gb-d
            ");


            var source = new XmlDirectoryLocalizationStorage(new[] { "localization1", "localization2", "localization3" });
            var factory = new LocalizationProviderFactory(source, new LocalizationMissingHandler(source, new CultureInfo("en-US")), new LocalizationCache());
            factory.LoadAll(x => { });
            factory.ApplyToLocalizationManager();

            var token = StringToken.FromKeyString("a", "Wrong!");

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            token.ToString().ShouldEqual("us-a");

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");

            token.ToString().ShouldEqual("gb-a");
        }
    }
}