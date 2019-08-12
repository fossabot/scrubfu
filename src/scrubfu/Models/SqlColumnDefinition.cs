/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿namespace Scrubfu.Models
{
    public sealed class SqlColumnDefinition
    {
        public string Name { get; set; }

        public string DataType { get; set; }

        public string Comment { get; set; }
    }
}