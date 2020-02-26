using NSubstitute;
using NUnit.Framework;
using Arg = NSubstitute.Arg;

namespace FubuCore.Testing
{
    [TestFixture]
    public class NumberExtensionsTester
    {
        private IAction _action;

        [SetUp]
        public void SetUp()
        {
            _action = Substitute.For<IAction>();
        }

        [Test]
        public void Times_runs_an_action_the_specified_number_of_times()
        {
            int maxCount = 6;
            maxCount.Times(_action.DoSomething);
            _action.Received(6).DoSomething(Arg.Is<int>(x => x < 6));
        }

        public interface IAction
        {
            void DoSomething(int index);
        }
    }
}