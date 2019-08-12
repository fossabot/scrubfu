/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿namespace Scrubfu.Models
{
    public sealed class SchemaNameParts
    {
        public string Schema { get; set; }

        public string Table { get; set; }

        public string TableFullName => string.Join(".", Schema, Table);

        public string Column { get; set; }

        public string ColumnFullName => string.Join(".", Schema, Table, Column);
    }
}