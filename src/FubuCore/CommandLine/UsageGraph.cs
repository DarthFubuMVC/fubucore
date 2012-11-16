using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;

namespace FubuCore.CommandLine
{
    public class UsageGraph
    {
        private readonly string _commandName;
        private readonly Type _commandType;
        private readonly IList<CommandUsage> _usages = new List<CommandUsage>();
        private string _description;
        private readonly Type _inputType;
        private readonly List<ITokenHandler> _handlers;
        private readonly string _appName;
        private readonly Lazy<IEnumerable<CommandUsage>> _validUsages; 

        public UsageGraph(Type commandType) : this("fubu", commandType)
        {
        }

        public UsageGraph(string appName, Type commandType)
        {
            _appName = appName;
            _commandType = commandType;
            _inputType = commandType.FindInterfaceThatCloses(typeof (IFubuCommand<>)).GetGenericArguments().First();

            _commandName = CommandFactory.CommandNameFor(commandType);
            _commandType.ForAttribute<CommandDescriptionAttribute>(att => { _description = att.Description; });

            if (_description == null) _description = _commandType.Name;

            _handlers = InputParser.GetHandlers(_inputType);

            _commandType.ForAttribute<UsageAttribute>(att =>
            {
                _usages.Add(buildUsage(att));
            });

            _validUsages = new Lazy<IEnumerable<CommandUsage>>(() => {
                if (_usages.Any()) return _usages;

                var usage = new CommandUsage()
                {
                    AppName = _appName,
                    CommandName = _commandName,
                    UsageKey = "default",
                    Description = _description,
                    Arguments = _handlers.OfType<Argument>(),
                    ValidFlags = _handlers.Where(x => !(x is Argument))
                };

                return new CommandUsage[]{usage};
            });
        }

        public object BuildInput(Queue<string> tokens)
        {
            var model = Activator.CreateInstance(_inputType);
            var responding = new List<ITokenHandler>();

            while (tokens.Any())
            {
                var handler = _handlers.FirstOrDefault(h => h.Handle(model, tokens));
                if (handler == null) throw new InvalidUsageException("Unknown argument or flag for value " + tokens.Peek());
                responding.Add(handler);
            }

            if (!IsValidUsage(responding))
            {
                throw new InvalidUsageException();
            }

            return model;
        }

        public bool IsValidUsage(IEnumerable<ITokenHandler> handlers)
        {
            return _validUsages.Value.Any(x => x.IsValidUsage(handlers));       
        }

        public IEnumerable<ITokenHandler> Handlers
        {
            get { return _handlers; }
        }

        private CommandUsage buildUsage(UsageAttribute att)
        {
            return new CommandUsage(){
                AppName = _appName,
                CommandName = _commandName,
                UsageKey = att.Name,
                Description = att.Description,
                Arguments = _handlers.OfType<Argument>().Where(x => x.RequiredForUsage(att.Name)),
                ValidFlags = _handlers.Where(x => x.OptionalForUsage(att.Name))
            };
        }

        public CommandUsage FindUsage(string key)
        {
            return _validUsages.Value.FirstOrDefault(x => x.UsageKey == key);
        }

        public string CommandName
        {
            get { return _commandName; }
        }

        public IEnumerable<Argument> Arguments
        {
            get
            {
                return _handlers.OfType<Argument>();
            }
        }
        public IEnumerable<ITokenHandler> Flags
        {
            get
            {
                return _handlers.Where(x => !(x is Argument));
            }
        }

        public IEnumerable<CommandUsage> Usages
        {
            get { return _validUsages.Value; }
        }

        public string Description
        {
            get { return _description; }
        }

        public void WriteUsages()
        {
            if (!Usages.Any())
            {
                Console.WriteLine("No documentation for this command");
                return;
            }

            Console.WriteLine(" Usages for '{0}' ({1})", _commandName, _description);

            if (Usages.Count() == 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" " + Usages.Single().Usage);
                Console.ResetColor();
            }
            else
            {
                writeMultipleUsages();
            }

            if(Arguments.Any())
                writeArguments();


            if (!Flags.Any()) return;

            writeFlags();
        }

        private void writeMultipleUsages()
        {
            var usageReport = new TwoColumnReport("Usages"){
                SecondColumnColor = ConsoleColor.Cyan
            };

            Usages.OrderBy(x => x.Arguments.Count()).ThenBy(x => x.ValidFlags.Count()).Each(u =>
            {
                usageReport.Add(u.Description, u.Usage);
            });

            usageReport.Write();
        }

        private void writeArguments()
        {
            var argumentReport = new TwoColumnReport("Arguments");
            Arguments.Each(x => argumentReport.Add(x.PropertyName.ToLower(), x.Description));
            argumentReport.Write();
        }

        private void writeFlags()
        {
            var flagReport = new TwoColumnReport("Flags");
            Flags.Each(x => flagReport.Add(x.ToUsageDescription(), x.Description));
            flagReport.Write();
        }
    }
}