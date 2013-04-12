﻿using System.Collections.Generic;
using System.Linq;

namespace FubuCore.CommandLine
{
    public class CommandUsage
    {
        public string Description { get; set; }
        public IEnumerable<Argument> Arguments { get; set; }
        public IEnumerable<ITokenHandler> ValidFlags { get; set; }

        public UsageReport ToReport(string appName, string commandName)
        {
            return new UsageReport
            {
                Description = Description,
                Usage = ToUsage(appName, commandName)
            };
        }

        public string ToUsage(string appName, string commandName)
        {
            return "{0} {1} {2}".ToFormat(appName, commandName,
                                          (Arguments.Cast<ITokenHandler>().Union(ValidFlags).Select(x => x.ToUsageDescription()))
                                              .Join(" "));
        }


        public bool IsValidUsage(IEnumerable<ITokenHandler> handlers)
        {
            var actualArgs = handlers.OfType<Argument>();
            if (actualArgs.Count() != Arguments.Count()) return false;

            if (!Arguments.All(x => actualArgs.Contains(x)))
            {
                return false;
            }

            var flags = handlers.Where(x => !(x is Argument));
            return flags.All(x => ValidFlags.Contains(x));
        }
    }
}