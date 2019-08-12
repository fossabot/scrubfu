/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
namespace Scrubfu.Tests.Models
{
    public class ScrubTestInput
    {
        public ScrubTestInput()
        {
        }

        public ScrubTestInput(string commandOptions, string inputStrings, string regExTestString)
        {
            CommandOptions = commandOptions;
            InputString = inputStrings;
            RegExTestString = regExTestString;
        }

        public string CommandOptions { get; set; }
        public string InputString { get; set; }
        public string RegExTestString { get; set; }
    }
}
