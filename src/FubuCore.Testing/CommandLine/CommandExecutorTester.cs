using FubuCore.CommandLine;
using NSubstitute;
using NUnit.Framework;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class CommandExecutorTester
    {
        private ICommandFactory factory;
        private IFubuCommand command;
        private CommandExecutor theExecutor;
        private object theInput;
        private string commandLine;

        [SetUp]
        public void SetUp()
        {
            factory = Substitute.For<ICommandFactory>();
            command = Substitute.For<IFubuCommand>();
            theInput = new object();
            commandLine = "some stuff here";

            theExecutor = new CommandExecutor(factory);
        }

        [Test]
        public void run_command_happy_path_executes_the_command_with_the_input()
        {
            factory.BuildRun(commandLine).Returns(new CommandRun(){
                Command = command,
                Input = theInput
            });

            theExecutor.Execute(commandLine);

            command.Received().Execute(theInput);
        }
    }
}