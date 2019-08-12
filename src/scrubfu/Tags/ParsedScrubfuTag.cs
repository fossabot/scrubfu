/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using Scrubfu.Contracts;

namespace Scrubfu.Tags
{
    public abstract class ParsedScrubfuTag : IParsedScrubfuTag
    {
        protected int priority;

        protected ParsedScrubfuTag(string optionsText = null, int? columnArrayIndex = null)
        {
            OptionsText = optionsText;
            ColumnArrayIndex = columnArrayIndex;
        }

        public virtual IScrubfuTagOptions Options => GetOptions();

        public string OptionsText { get; set; }

        public int? ColumnArrayIndex { get; set; }

        public virtual IScrubfuTagOptions GetOptions(string optionsText = null)
        {
            throw new NotImplementedException();
        }

        public virtual string Apply(string fieldText, IScrubfuTagOptions commandOptions = null)
        {
            throw new NotImplementedException();
        }

        public virtual bool ValidateTag()
        {
            return false;
        }
    }
}