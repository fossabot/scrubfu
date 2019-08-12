/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿
using Scrubfu.Contracts;

namespace Scrubfu.Tags
{
    public sealed class FuzzTagOptions : IScrubfuTagOptions
    {
        public bool ValidateOptions()
        {
            return true;
        }
    }
}