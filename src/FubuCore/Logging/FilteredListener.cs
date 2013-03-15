using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Logging
{
    public abstract class FilteredListener<TListener>
    {
        private readonly Level _level;
        private readonly IList<Func<Type, bool>> _filters = new List<Func<Type, bool>>();

        protected FilteredListener(Level level)
        {
            _level = level;
        }

        protected abstract TListener thisInstance();

        public bool ListensFor(Type type)
        {
            return !_filters.Any() || _filters.Any(x => x(type));
        }

        public bool IsDebugEnabled
        {
            get { return _level == Level.All || _level == Level.DebugOnly; }
        }

        public bool IsInfoEnabled
        {
            get { return _level == Level.All || _level == Level.InfoOnly; }
        }

        public TListener ListenFor<T>()
        {
            return ListenFor(typeof(T));
        }

        public TListener ListenFor(Type type)
        {
            return ListenFor(t => t == type);
        }

        public TListener ListenForAnythingImplementing<T>()
        {
            return ListenFor(t => t.CanBeCastTo<T>());
        }

        public TListener ListenFor(Func<Type, bool> filter)
        {
            _filters.Add(filter);
            return thisInstance();
        }
    }
}