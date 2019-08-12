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
    public sealed class MaskTagOptions : IScrubfuTagOptions
    {
        public string[] IgnoreStrings { get; set; } = { };
        public char MaskChar { get; set; } = '#';
        public int MaskEndOffset { get; set; } = 0;
        public int MaskStartOffset { get; set; } = 0;

        public bool ValidateOptions()
        {
            return true;
        }

    }
}