using System;

namespace FubuCore.CommandLine
{
    public class ActivatorCommandCreator : ICommandCreator
    {
        public IFubuCommand Create(Type commandType)
        {
            return Activator.CreateInstance(commandType).As<IFubuCommand>();
        }
    }
}
