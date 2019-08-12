/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System.Collections.Generic;

namespace Scrubfu.Models
{
    public sealed class SqlCommandInfo
    {
        public string Command { get; set; }
        public SqlCommandType Type { get; set; } = 0;
    }

    public enum SqlCommandType
    {
        UNKNOWN = 0,
        DDL = 1,
        DML = 2
    }
}