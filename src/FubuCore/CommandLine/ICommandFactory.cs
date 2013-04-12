using System.Collections.Generic;
using System.Reflection;

namespace FubuCore.CommandLine
{
    public interface ICommandFactory
    {
        CommandRun BuildRun(string commandLine);
        CommandRun BuildRun(IEnumerable<string> args);
        void RegisterCommands(Assembly assembly);

        IEnumerable<IFubuCommand> BuildAllCommands();
    }
}