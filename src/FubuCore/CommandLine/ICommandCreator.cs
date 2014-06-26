using System;

namespace FubuCore.CommandLine
{
    public interface ICommandCreator
    {
        IFubuCommand Create(Type commandType);
    }
}
