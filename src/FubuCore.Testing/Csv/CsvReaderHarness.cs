using System.Collections.Generic;
using System.IO;
using FubuCore.Binding;
using FubuCore.Csv;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Csv
{
    public class CsvReaderHarness<TMap, T>
        where TMap : ColumnMapping<T>, new()
    {
        private string theFile;
        private CsvReader theReader;
        private CsvRequest<T> theRequest;
        protected IList<T> theResults;
            
        [SetUp]
        public void SetUp()
        {
            theFile = "{0}.csv".ToFormat(GetType().Name);

            using (var writer = new StreamWriter(theFile))
            {
                writeFile(writer);
            }

            theReader = new CsvReader(ObjectResolver.Basic());

            theResults = new List<T>();

            theRequest = new CsvRequest<T>
                         {
                             Mapping = new TMap(),
                             FileName = theFile,
                             Callback = theResults.Add
                         };

            beforeEach();

            configureRequest(theRequest);

            theReader.Read(theRequest);
        }

        protected virtual void beforeEach()
        {
        }

        protected virtual void writeFile(StreamWriter writer)
        {   
        }

        protected virtual void configureRequest(CsvRequest<T> request)
        {
        }

        protected void theResultsAre(params T[] expected)
        {
            theResults.ShouldHaveTheSameElementsAs(expected);
        }
    }
}