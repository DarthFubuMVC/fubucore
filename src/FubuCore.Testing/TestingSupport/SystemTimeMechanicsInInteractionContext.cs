using System;
using FubuCore.Dates;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.TestingSupport
{
    [TestFixture]
    public class SystemTimeMechanicsInInteractionContext : InteractionContext<ThingThatUsesSystemTime>
    {
        private DateTime systemTime;

        protected override void beforeEach()
        {
            systemTime = DateTime.Today.AddHours(3);
            LocalSystemTime = systemTime;
        }

        [Test]
        public void the_time_was_set_in_the_beforeEach()
        {
            ClassUnderTest.SystemTime.LocalNow().ShouldEqual(systemTime);
        }
    }

    public class ThingThatUsesSystemTime
    {
        private readonly ISystemTime _systemTime;

        public ThingThatUsesSystemTime(ISystemTime systemTime)
        {
            _systemTime = systemTime;
        }

        public ISystemTime SystemTime
        {
            get { return _systemTime; }
        }
    }
}