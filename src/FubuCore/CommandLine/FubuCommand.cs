using System;

namespace FubuCore.CommandLine
{
    public abstract class FubuCommand<T> : IFubuCommand<T>
    {
        private readonly UsageGraph _usages;

        protected FubuCommand()
        {
            _usages = new UsageGraph(GetType());
        }

        public UsageGraph Usages
        {
            get { return _usages; }
        }

        public Type InputType
        {
            get
            {
                return typeof (T);
            }
        }

        public bool Execute(object input)
        {
            return Execute((T)input);
        }

        public abstract bool Execute(T input);
    }
}