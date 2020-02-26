using System;
using FubuCore.Logging;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Logging
{
    [TestFixture]
    public class LogRecordModifierTester : NSubstituteInteractionContext<LogRecordModifier>
    {
        [Test]
        public void matches_only_log_record_things()
        {
            ClassUnderTest.Matches(typeof(string)).ShouldBeFalse();
            ClassUnderTest.Matches(GetType()).ShouldBeFalse();
            ClassUnderTest.Matches(typeof(LogRecord)).ShouldBeFalse();
        
            ClassUnderTest.Matches(typeof(FakeLogRecord1)).ShouldBeTrue();
            ClassUnderTest.Matches(typeof(FakeLogRecord2)).ShouldBeTrue();
        }

        [Test]
        public void modify()
        {
            LocalSystemTime = DateTime.Today.AddHours(7);

            var record = new FakeLogRecord1();

            ClassUnderTest.Modify(record);

            record.Time.ShouldEqual(UtcSystemTime);
        }
    }

    public class FakeLogRecord1 : LogRecord{}
    public class FakeLogRecord2 : LogRecord{}
}