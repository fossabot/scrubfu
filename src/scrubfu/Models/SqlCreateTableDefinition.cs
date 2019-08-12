/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System.Collections.Generic;

namespace Scrubfu.Models
{
    public sealed class SqlCreateTableDefinition
    {
        public SchemaNameParts Name { get; set; }

        public List<SqlColumnDefinition> Columns { get; set; } = new List<SqlColumnDefinition>();
    }
}