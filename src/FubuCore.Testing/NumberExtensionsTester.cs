using Moq;
using NUnit.Framework;
using FubuCore;

namespace FubuCore.Testing
{
    [TestFixture]
    public class NumberExtensionsTester
    {
        private Mock<IAction> _action;

        [SetUp]
        public void SetUp()
        {
            _action = new Mock<IAction>();
            _action.Setup(a => a.DoSomething(It.IsInRange<int>(0, 6, Range.Inclusive)));
        }

        [Test]
        public void Times_runs_an_action_the_specified_number_of_times()
        {
            int maxCount = 6;
            maxCount.Times(_action.Object.DoSomething);
            _action.Verify(a => a.DoSomething(It.IsInRange<int>(0, 6, Range.Inclusive)), Times.Exactly(6));
        }

        public interface IAction
        {
            void DoSomething(int index);
        }
    }
}