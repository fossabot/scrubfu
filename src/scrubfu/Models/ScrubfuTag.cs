/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
using Scrubfu.Tags;

namespace Scrubfu.Models
{
    public sealed class ScrubfuTag
    {
        public string SchemaEntityName { get; set; }

        public ParsedScrubfuTag Tag { get; set; }
    }
}