using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Logging
{
    public class ListenerCollection : IEnumerable<ILogListener>
    {
        private readonly IEnumerable<ILogListener> _listeners;
        private readonly IEnumerable<ILogModifier> _modifiers;

        public ListenerCollection(IEnumerable<ILogListener> listeners, IEnumerable<ILogModifier> modifiers)
        {
            _listeners = listeners;
            _modifiers = modifiers;
        }

        private Action<Func<T>> findAction<T>(Func<ILogListener, bool> filter, Func<ILogModifier, bool> modifierFilter, Action<ILogListener, T> proceed)
        {
            var listeners = _listeners.Where(filter).ToList();
            if (listeners.Any())
            {
                var modifiers = _modifiers.Where(modifierFilter).ToArray();

                return source =>
                {
                    try
                    {
                        var msg = source();
                        modifiers.Each(x => x.Modify(msg));

                        listeners.Each(x =>
                        {
                            try
                            {
                                proceed(x, msg);
                            }
                            catch (Exception e)
                            {
                                // It's just logging
                                Console.WriteLine(e);
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };

                


            }

            return msg => { };
        }

        public Action<Func<string>> Debug()
        {
            return findAction<string>(x => x.IsDebugEnabled, x => false, (listener, msg) => listener.Debug(msg));
        }

        public Action<Func<string>> Info()
        {
            return findAction<string>(x => x.IsInfoEnabled, x => false, (listener, msg) => listener.Info(msg));
        }

        public Action<Func<object>> DebugFor(Type type)
        {
            return findAction<object>(x => x.IsDebugEnabled && x.ListensFor(type), x => x.Matches(type), (l, o) => l.DebugMessage(o));
        }

        public Action<Func<object>> InfoFor(Type type)
        {
            return findAction<object>(x => x.IsInfoEnabled && x.ListensFor(type), x => x.Matches(type), (l, o) => l.InfoMessage(o));
        }

        public IEnumerator<ILogListener> GetEnumerator()
        {
            return _listeners.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}