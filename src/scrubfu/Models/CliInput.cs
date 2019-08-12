/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System.Collections.Generic;
using System.Linq;

namespace Scrubfu.Models
{
    public class CliInput
    {
        public List<CliOption> options { get; set; }
        public string InFile { get; set; }
        public string InContents { get; set; }
        public string OutFile { get; set; }
        public string OutContents { get; set; }

        public CliInput()
        {
            options = new List<CliOption>();
        }

        public bool AskingVersion
        {
            get
            {
                return (options.Any(x => (x.Name == Constants.VersionCliCommandOption || x.Name == Constants.VersionCliCommandOptionShortHand)));
            }
        }

        public bool AskingHelp
        {
            get
            {
                return (options.Any(x => (x.Name == Constants.HelpCliCommandOption || x.Name == Constants.HelpCliCommandOptionShortHand)));
            }
        }

        public bool SpecifyingLogFile
        {
            get
            {
                return (options.Any(x => (x.Name == Constants.LogCliCommandOption || x.Name == Constants.LogCliCommandOptionShortHand)));
            }
        }

        public bool SpecifyingLogLevel
        {
            get
            {
                return (options.Any(x => (x.Name == Constants.LogLevelCliCommandOption)));
            }
        }

        public string LogfileLocation
        {
            get
            {
                return (options.FirstOrDefault(x => x.Name == Constants.LogCliCommandOption || x.Name == Constants.LogCliCommandOptionShortHand).Value);
            }
        }

        public string Loglevel
        {
            get
            {
                CliOption logLevelOption = options.FirstOrDefault(x => x.Name == Constants.LogLevelCliCommandOption);

                if (logLevelOption != null)
                    return (logLevelOption.Value);
                else
                    return "info";
            }
        }
    }
}
