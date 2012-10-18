using System;
using System.IO;
using FubuCore.Csv;
using FubuCore.Reflection;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    [TestFixture]
    public abstract class SpecialCharacterHarness : CsvReaderHarness<TestCsvDescriptionMapping, TestCsvObject>
    {
        protected abstract string getActualContent();

        protected virtual string getExpectedContent()
        {
            return getActualContent();
        }

        protected override void writeFile(StreamWriter writer)
        {
            writer.WriteLine("\"{0}\"".ToFormat(getActualContent()));
        }

        protected override void configureRequest(CsvRequest<TestCsvObject> request)
        {
            base.configureRequest(request);
            request.HeadersExist = false;
        }

        [Test]
        public void theDescriptionMatchesTheValue()
        {
            theResultsAre(new TestCsvObject { Description = getExpectedContent() });
        }
    }

    [TestFixture]
    public abstract class SpecialCharacterHeaderHarness : CsvReaderHarness<TestCsvDescriptionMapping, TestCsvObject>
    {
        private string theValue;

        protected abstract string getActualContent();

        protected virtual string getExpectedContent()
        {
            return getActualContent();
        }

        protected override void writeFile(StreamWriter writer)
        {
            writer.WriteLine("\"{0}\"".ToFormat(getActualContent()));
            writer.WriteLine(theValue = Guid.NewGuid().ToString());
        }

        protected override void configureRequest(CsvRequest<TestCsvObject> request)
        {
            base.configureRequest(request);
            request.Mapping.As<TestCsvDescriptionMapping>().Alias(getExpectedContent());
        }

        [Test]
        public void theDescriptionMatchesTheValue()
        {
            theResultsAre(new TestCsvObject { Description = theValue });
        }
    }

    public class TestCsvDescriptionMapping : ColumnMapping<TestCsvObject>
    {
        public TestCsvDescriptionMapping()
        {
            Column(x => x.Description);
        }

        public void Alias(string alias)
        {
            var def = this.As<IColumnMapping>()
                .ColumnFor(ReflectionHelper.GetAccessor<TestCsvObject>(x => x.Description));
            new ColumnExpression(def)
                .Alias(alias);
        }
    }
}