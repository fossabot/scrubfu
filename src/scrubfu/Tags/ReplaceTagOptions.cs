/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System.Collections.Generic;
using Scrubfu.Contracts;

namespace Scrubfu.Tags
{
    public sealed class ReplaceTagOptions : IScrubfuTagOptions
    {
        public Dictionary<string, string> Replacements { get; set; } = new Dictionary<string, string>();

        public bool ValidateOptions()
        {
            return true;
        }

    }
}