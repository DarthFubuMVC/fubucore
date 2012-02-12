using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class RequestDataTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            dictionary = new Dictionary<string, object>();
            aggregate = new AggregateDictionary();
            aggregate.AddDictionary("Other", dictionary);


            request = new RequestData(aggregate);
        }

        #endregion

        private Dictionary<string, object> dictionary;
        private RequestData request;
        private AggregateDictionary aggregate;

        private const string _expectedValue = "STUBBED USERAGENT";

        public interface ICallback
        {
            void Action(object o);
        }

        [Test]
        public void call_into_the_continuation_with_the_dictionary_value_if_it_exists()
        {
            string name = null;

            dictionary.Add("name", "Something");

            request.Value("name", o => name = (string) o.RawValue);

            name.ShouldEqual("Something");
        }

        [Test]
        public void do_nothing_when_the_value_is_not_found()
        {
            request.Value("name", o => { throw new NotImplementedException(); });
        }

        [Test]
        public void for_dictionary_returns_new_request_data_with_added_dictionary()
        {
            var callback = MockRepository.GenerateStub<ICallback>();
            IDictionary<string, object> dictionary = new Dictionary<string, object>{
                {"UserAgent", _expectedValue}
            };
            var data = RequestData.ForDictionary(dictionary);
            data.ShouldNotBeNull();
            data.Value("UserAgent", callback.Action);


            callback.AssertWasCalled(c => c.Action(new RequestSource{
                RawKey = "UserAgent",
                RawValue = _expectedValue,
                Source = "Other"
            }));
        }

        [Test]
        public void value_returns_value_for_key()
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>{
                {"UserAgent", _expectedValue}
            };
            var data = RequestData.ForDictionary(dictionary);
            data.Value("UserAgent").ShouldEqual(_expectedValue);
        }
    }
}