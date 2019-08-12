/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System.Collections.Generic;
using System.Linq;
using Scrubfu.Models;

namespace Scrubfu
{
    public static class Constants
    {
        public const string HelpCliCommandOption = "--help";
        public const string HelpCliCommandOptionShortHand = "-h";

        public const string VersionCliCommandOption = "--version";
        public const string VersionCliCommandOptionShortHand = "-v";

        public const string LogCliCommandOption = "--log";
        public const string LogCliCommandOptionShortHand = "-ll";

        public const string LogLevelCliCommandOption = "--log_level";
        public const string LogLevelCliCommandOptionValues = "error|info|debug";

        public const string Char_Tab = "\t";

        public const string SINGLE_QUOTE = "'";
        public const string SINGLE_SPACE = " ";
        public const string TILDE = "~";
        public const string FLAG_COMMENT_ON_COLUMN = "COMMENT ON COLUMN";
        public const string FLAG_COMMENT_ON_COLUMN_END_DELIMITER = "';";
        public const string FLAG_SPACE_IS_SPACE = " IS ";
        public const string FLAG_SQL_COMMENT_DELIMITER = "--";

        public const string SqlCommand_CREATE_TABLE = "CREATE TABLE";
        public const string SqlCommand_INSERT_INTO = "INSERT INTO";
        public const string SqlCommand_INSERT_Terminator = ")";
        public const string SqlCommand_UPDATE = "UPDATE";
        public const string SqlCommand_COPY = "COPY";
        public const string SqlCommand_COPY_TERMINATOR = @"\.";

        public const string SqlCommand_INSERT_INTO_FULL_REGEX = @"insert into(.)*\)( )* values() *\((.|\n)*\)( )*;";
        public const string SqlCommand_INSERT_INTO_PARTIAL_REGEX = @"insert into(.)* values( )*(.|\n)*\)( )*;";
        public const string SqlCommand_INSERT_VALUES_REGEX = @"values() *\((.|\n)*\)( )*;";
        public const int POSTGRESS_MAX_TABLE_COLUMNS_REASONABLE = 1600;
        private static List<SqlCommandInfo> sqlCommands = new List<SqlCommandInfo>
        {
            new SqlCommandInfo {Command = SqlCommand_CREATE_TABLE, Type = SqlCommandType.DDL},
            new SqlCommandInfo {Command = SqlCommand_INSERT_INTO, Type = SqlCommandType.DML},
            new SqlCommandInfo {Command = SqlCommand_UPDATE, Type = SqlCommandType.DML},
            new SqlCommandInfo {Command = SqlCommand_COPY, Type = SqlCommandType.DML}
        };

        public static List<SqlCommandInfo> SqlCommands => sqlCommands.OrderBy(s => s.Command).ToList();

        public static List<string> GetValidCommands()
        {
            return new List<string>()
            {
                HelpCliCommandOption,
                HelpCliCommandOptionShortHand,
                VersionCliCommandOption,
                VersionCliCommandOptionShortHand,
                LogCliCommandOption,
                LogCliCommandOptionShortHand,
                LogLevelCliCommandOption
            };
        }

        public static List<string> GetValidCommandsRequiringValues()
        {
            return new List<string>()
            {
                LogCliCommandOption,
                LogCliCommandOptionShortHand,
                LogLevelCliCommandOption
            };
        }
    }
}