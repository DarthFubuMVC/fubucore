using FubuCore.CommandLine;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class CommandExecutorTester
    {
        private Mock<ICommandFactory> factory;
        private Mock<IFubuCommand> command;
        private CommandExecutor theExecutor;
        private object theInput;
        private string commandLine;

        [SetUp]
        public void SetUp()
        {
            factory = new Mock<ICommandFactory>();
            command = new Mock<IFubuCommand>();
            theInput = new object();
            commandLine = "some stuff here";

            theExecutor = new CommandExecutor(factory.Object);
        }

        [Test]
        public void run_command_happy_path_executes_the_command_with_the_input()
        {
            factory.Setup(x => x.BuildRun(commandLine)).Returns(new CommandRun(){
                Command = command.Object,
                Input = theInput
            });

            theExecutor.Execute(commandLine);

            command.Verify(x => x.Execute(theInput));
        }
    }
}