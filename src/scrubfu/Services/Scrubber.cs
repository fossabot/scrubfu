using System.Reflection.Metadata;
/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scrubfu.Models;
using Scrubfu.Enums;
using Scrubfu.Extensions;
using Scrubfu.Tags;
using Scrubfu.Exceptions;
using System.Text;
using System.Text.RegularExpressions;
using NLog;

namespace Scrubfu.Services
{
    public sealed class Scrubber
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private readonly TextReader inReader;
        private readonly TextWriter outWriter;
        private readonly List<SqlCreateTableDefinition> tableDefinitions;

        private readonly List<string> errors;
        public List<string> Errors
        {
            get 
            {
                return errors;
            }
        }

        private readonly List<string> warnings;
        public List<string> Warnings
        {
            get 
            {
                return warnings;
            }
        }

        public Scrubber(TextReader inReader, TextWriter outWriter)
        {
            this.inReader = inReader;
            this.outWriter = outWriter;
            this.errors = new List<string>();
            this.warnings = new List<string>();

            tableDefinitions = new List<SqlCreateTableDefinition>();
        }

        public void Scrub()
        {
            string line;
            int lineNum = 0;

            while ((line = inReader.ReadLine()) != null)
            {
                lineNum++;
                outWriter.Flush();

                if (line.StartsWith(Constants.SqlCommand_CREATE_TABLE, StringComparison.OrdinalIgnoreCase))  // Table definition
                {   
                    logger.Debug(string.Format("Processing table definition. [{0}]", line));
                    tableDefinitions.Add(ProcessTableDefinition(line, ref lineNum));
                    continue;
                }

                if (line.StartsWith(Constants.FLAG_COMMENT_ON_COLUMN, StringComparison.OrdinalIgnoreCase))  // Column comment definition
                {
                    logger.Debug(string.Format("Processing comment line. [{0}]", line));
                    ProcessCommentLine(line, ref lineNum);
                    continue;
                }

                if (line.StartsWith(Constants.SqlCommand_INSERT_INTO, StringComparison.OrdinalIgnoreCase))  // Insert Data line
                {
                    logger.Debug(string.Format("Processing insert into line. [{0}]", line));
                    ProcessInsertStatement(line, ref lineNum);
                    continue;
                }

                if (line.StartsWith(Constants.SqlCommand_COPY, StringComparison.OrdinalIgnoreCase))  // Copy Data line
                {
                    logger.Debug(string.Format("Processing copy line. [{0}]", line));
                    ProcessCopyStatement(line, ref lineNum);
                    continue;
                }

                // Everything else just copy out
                outWriter.WriteLine(line);
            }

            outWriter.Flush();
        }


        #region "Line processing methods"
        private SqlCreateTableDefinition ProcessTableDefinition(string initialLine, ref int lineNum)
        {
            var tableSql = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(initialLine))
            {
                outWriter.WriteLine(initialLine);
                tableSql.AppendLine(initialLine);
            }

            int timeout = 0;
            while (!tableSql.ToString().EndsWith(";", StringComparison.Ordinal)
                && !tableSql.ToString().EndsWith(string.Concat(";", Environment.NewLine), StringComparison.Ordinal)
                && (timeout++ <= Constants.POSTGRESS_MAX_TABLE_COLUMNS_REASONABLE))
            {
                string line = inReader.ReadLine();
                lineNum++;

                if (line == null)
                    break;

                outWriter.WriteLine(line);
                tableSql.AppendLine(line);
            }

            return ParseSqlCreateTableStatement(tableSql.ToString());
        }

        private void ProcessCommentLine(string line, ref int lineNum)
        {
            string initialLine = line;

            try
            {
                int colNameStart = Constants.FLAG_COMMENT_ON_COLUMN.Length + 1;
                int colNameEnd = line.IndexOf(Constants.SINGLE_SPACE, colNameStart, StringComparison.Ordinal);

                string fullColumnName = line.Substring(colNameStart, colNameEnd - colNameStart).Trim();
                string skipString = $"{Constants.FLAG_COMMENT_ON_COLUMN} {fullColumnName} IS ";

                string tableName = fullColumnName.Substring(0, fullColumnName.LastIndexOf(".", StringComparison.Ordinal));
                string columnName = fullColumnName.Substring(fullColumnName.LastIndexOf(".", StringComparison.Ordinal) + 1);
                string comment = line.Substring(skipString.Length, line.Length - skipString.Length - 1).Trim();

                if (comment.IsWrappedByString("'"))
                    comment = comment.Substring(1, comment.Length - 2);
                    
                tableDefinitions.FirstOrDefault(x => x.Name.TableFullName == tableName).Columns.FirstOrDefault(y => y.Name == columnName).Comment = comment;

                outWriter.WriteLine(line);
            }
            catch (Exception ex)
            {
                // Log error, then write file to output as is
                LogError(string.Format("Error processing comment line on line {0}. {1}", lineNum, ex.ExtractEntireErrorMessage()));
                outWriter.WriteLine(initialLine);
            }
        }

        private void ProcessInsertStatement(string line, ref int lineNum)
        {
            int timeout = 0;
            while (!CheckValidInsertIntoStatement(line) && (timeout++ < 100))
            {
                string nextInsertLine = inReader.ReadLine();
                lineNum++;

                if (nextInsertLine == null)
                    break;

                line = string.Concat(line, Environment.NewLine, nextInsertLine);
            }

            var tableName = ParseEntityName(Constants.SqlCommand_INSERT_INTO, line);
            var tableActionList = GetScrubfuTagsForTable(tableName, lineNum);

            if (tableActionList.Count == 0)
            {
                outWriter.WriteLine(line);
                return;
            }

            ApplyTagsForInsert(line, tableName, tableActionList, tableDefinitions.FirstOrDefault(d => d.Name.TableFullName == tableName), ref lineNum);
        }

        private void ProcessCopyStatement(string line, ref int lineNum)
        {
            var tableName = ParseEntityName(Constants.SqlCommand_COPY, line);
            var tableActionList = GetScrubfuTagsForTable(tableName, lineNum);

            if (tableActionList.Count == 0)
            {
                outWriter.WriteLine(line);
                return;
            }

            ApplyTagsForCopy(line, tableName, tableActionList, tableDefinitions.FirstOrDefault(d => d.Name.TableFullName == tableName), ref lineNum);
        }
        #endregion


        #region "Tag extraction methods
        private List<ScrubfuTag> GetScrubfuTagsForTable(string tableName, int lineNum)
        {
            List<ScrubfuTag> tableTags = new List<ScrubfuTag>();

            SqlCreateTableDefinition lookupTable = tableDefinitions.FirstOrDefault(x => x.Name.TableFullName == tableName);

            if (lookupTable != null)
            {
                lookupTable.Columns.Where(x => !string.IsNullOrWhiteSpace(x.Comment.Trim())).ToList()
                .ForEach(y =>
                {
                    List<ParsedScrubfuTag> tags = GetScrubfuTagsFromColumnComment(y.Comment, lineNum);

                    foreach (var tag in tags)
                    {
                        tableTags.Add(new ScrubfuTag()
                        {
                            SchemaEntityName = string.Concat(lookupTable.Name.TableFullName, ".", y.Name),
                            Tag = tag
                        });
                    }
                });
            }

            return tableTags;
        }

        private List<string> GetTagSetsInComment(string comment, int lineNum)
        {
            List<string> tildeSets = new List<string>();
            int tagStartPosition = -1;
            bool insideSingleQuotes = false;
            bool insideDoubleQuotes = false;
            bool escapeNextChar = false;

            for (int charPos = 0; charPos < comment.Length; charPos++)
                ProcessTagChar(comment, charPos, ref tildeSets, ref tagStartPosition, ref insideSingleQuotes, ref insideDoubleQuotes, ref escapeNextChar);

            ProcessTagWarnings(comment, tagStartPosition, lineNum);

            return tildeSets;
        }

        private void ProcessTagChar(string comment, int charPos, ref List<string> tildeSets, ref int tagStartPosition, ref bool insideSingleQuotes, ref bool insideDoubleQuotes, ref bool escapeNextChar)
        {
            char readChar = comment[charPos];

            switch (readChar)
            {
                case '~':
                    ProcessTildeCharForTag(comment, charPos, ref tildeSets, ref tagStartPosition, ref insideSingleQuotes, ref insideDoubleQuotes, ref escapeNextChar);
                    break;

                case '\'':
                    ProcessSingleQuoteCharForTag(comment, charPos, ref insideSingleQuotes, ref escapeNextChar);
                    break;

                case '"':
                    if (!escapeNextChar)
                        insideDoubleQuotes = !insideDoubleQuotes;

                    escapeNextChar = false;
                    break;

                case '\\': 
                    if (!escapeNextChar)
                        escapeNextChar = true;

                    break;
                default:
                    escapeNextChar = false;
                    break;
            }
        }

        private void ProcessTildeCharForTag(string comment, int charPos, ref List<string> tildeSets, ref int tagStartPosition, ref bool insideSingleQuotes, ref bool insideDoubleQuotes, ref bool escapeNextChar)
        {
            if (tagStartPosition == -1)
                tagStartPosition = charPos;
            else if (!insideDoubleQuotes && !insideSingleQuotes && !escapeNextChar)
            {
                if (charPos - tagStartPosition > 1) 
                {
                    tildeSets.Add(comment.Substring(tagStartPosition + 1, charPos - tagStartPosition - 1));
                    tagStartPosition = -1;
                }
                else    // Don't use empty tilde set
                    tagStartPosition = -1;
            }
            else
                escapeNextChar = false;
        }

        private void ProcessSingleQuoteCharForTag(string comment, int charPos, ref bool insideSingleQuotes, ref bool escapeNextChar)
        {
            if (!escapeNextChar)
                insideSingleQuotes = !insideSingleQuotes;

            if ((charPos < comment.Length - 1) && (comment[charPos + 1] == '\'') && !escapeNextChar)
                escapeNextChar = true;
            else 
                escapeNextChar = false;
        }

        private void ProcessTagWarnings(string comment, int tagStartPosition, int lineNum)
        {
            if (tagStartPosition != -1) // Tilde was not closed. Might be a mistake, so warn.
                LogWarning(string.Format("An unescaped or unclosed tilde was found in comment while processing line {0}. This might indicate an incorrect Scrubfu tag definition. Comment: [{1}]", lineNum, comment));
        }

        private List<ParsedScrubfuTag> GetScrubfuTagsFromColumnComment(string comment, int lineNum)
        {
            List<ParsedScrubfuTag> returnTags = new List<ParsedScrubfuTag>();

            List<string> tagDefinitions = GetTagSetsInComment(comment, lineNum);

            for (int i = 0; i < tagDefinitions.Count; i++)
            {
                string tagDefinition = tagDefinitions[i];

                var tagName = tagDefinition;
                int? columnArrayIndex = null;
                var tagOptions = "";

                if (tagDefinition.IndexOf(":", StringComparison.Ordinal) > -1)
                {
                    tagName = tagDefinition.Substring(0, tagDefinition.IndexOf(":", StringComparison.Ordinal));

                    if (tagName.Contains("[") && tagName.Contains("]"))
                    {
                        columnArrayIndex = int.Parse(tagName.Substring(tagName.IndexOf("[", StringComparison.Ordinal) + 1,
                            tagName.IndexOf("[", StringComparison.Ordinal) - 1));
                        tagName = tagName.Substring(0, tagName.IndexOf("[", StringComparison.Ordinal));
                    }

                    tagOptions = tagDefinition.Substring(tagDefinition.IndexOf(":", StringComparison.Ordinal) + 1);
                }

                ParsedScrubfuTag tag = GetParsedTagFromTagString(tagName, tagOptions, columnArrayIndex, lineNum);

                if (tag != null)
                    returnTags.Add(tag);
            }

            return returnTags;
        }

        private ParsedScrubfuTag GetParsedTagFromTagString(string tagName, string tagOptions, int? columnArrayIndex, int lineNum)
        {
            ParsedScrubfuTag tag = null;
            
            switch (tagName.ToUpper())
            {
                case "MA":
                case "MASK":
                    tag = new MaskTag(tagOptions, columnArrayIndex);
                    break;

                case "RE":
                case "REPLACE":
                    tag = new ReplaceTag(tagOptions, columnArrayIndex);
                    break;

                case "RA":
                case "RANDOM":
                    tag = new RandomTag(tagOptions, columnArrayIndex);
                    break;

                case "FZ":
                case "FUZZ":
                    tag = new FuzzTag(tagOptions, columnArrayIndex);
                    break;
            }

            if (tag != null && !tag.ValidateTag())
            {
                LogWarning(string.Format("A potentially invalidly formatted Scrubfu tag was found while processing line {0}.", lineNum));
                tag = null;
            }

            return tag;
        }
        #endregion


        #region "Tag action application methods

        private void ApplyTagsForInsert(string lineText, string entityName, List<ScrubfuTag> entityActionList, SqlCreateTableDefinition tableDefinition, ref int lineNum)
        {
            string unmodifiedLine = lineText;
            
            try
            {
                if (string.IsNullOrEmpty(lineText))
                    return;

                string[] entityColumnList = ParseDefinedColumns(Constants.SqlCommand_INSERT_INTO, lineText);
                string[] columnValues = ParseInsertStatementValues(lineText);

                ApplyActions(entityName, entityColumnList, entityActionList, columnValues, tableDefinition, Constants.SqlCommand_INSERT_INTO);
                lineText = ReplaceInsertStatementValues(lineText, columnValues);

                outWriter.WriteLine(lineText);
            }
            catch (Exception ex)
            {
                // Log error, then write file to output as is
                LogError(string.Format("Error processing INSERT INTO statement on line {0}. {1}", lineNum, ex.ExtractEntireErrorMessage()));
                outWriter.WriteLine(unmodifiedLine);
            }
        }

        private void LogError(string errorString)
        {
            logger.Error(errorString);
            errors.Add(errorString);
        }

        private void LogWarning(string warningString)
        {
            logger.Warn(warningString);
            warnings.Add(warningString);
        }

        private void ApplyTagsForCopy(string lineText, string entityName, List<ScrubfuTag> entityActionList, SqlCreateTableDefinition tableDefinition, ref int lineNum)
        {
            string unmodifiedLine = lineText;
            
            try
            {
                // Output the copy header with column names
                outWriter.WriteLine(lineText);

                var entityColumnList = ParseDefinedColumns(Constants.SqlCommand_COPY, lineText);

                // Process each line of the copy block
                while (true)
                {
                    try
                    {
                        unmodifiedLine = lineText = inReader.ReadLine();
                        lineNum++;

                        if (string.IsNullOrEmpty(lineText))
                            continue;

                        if (lineText.StartsWith(Constants.SqlCommand_COPY_TERMINATOR, StringComparison.Ordinal))        // End of copy block was reached
                        {
                            outWriter.WriteLine(lineText);
                            break;
                        }

                        var columnValues = lineText.SplitWithValues(Constants.Char_Tab);
                        ApplyActions(entityName, entityColumnList, entityActionList, columnValues, tableDefinition, Constants.SqlCommand_COPY);
                        outWriter.WriteLine(string.Join(Constants.Char_Tab, columnValues));
                    }
                    catch (Exception ex)
                    {
                        // Log error, then write file to output as is
                        LogError(string.Format("Error processing COPY line entry on line {0}. {1}", lineNum, ex.ExtractEntireErrorMessage()));
                        outWriter.WriteLine(unmodifiedLine);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error, then write file to output as is
                LogError(string.Format("Error processing COPY statement on line {0}. {1}", lineNum, ex.ExtractEntireErrorMessage()));
                outWriter.WriteLine(unmodifiedLine);
            }
        }

        private void ApplyActions(string entityName, string[] entityColumnList, List<ScrubfuTag> entityActionList, string[] columnValues, SqlCreateTableDefinition tableDefinition, string sqlCommand)
        {
            foreach (var action in entityActionList)
            {
                var schemaEntityNameParts = action.SchemaEntityName.ToColumnSchemaParts();

                if (schemaEntityNameParts.TableFullName != entityName) continue;

                var columnPosition = Array.IndexOf(entityColumnList, schemaEntityNameParts.Column);
                var columnDataType = GetColumnDataType(tableDefinition, schemaEntityNameParts.Column);

                // If we're scrubbing an item in a one dimensional array, in a field of an array data type.
                if (columnDataType == SqlFieldDataType.Array)
                    columnValues[columnPosition] = ApplyActionsToArray(action, columnValues[columnPosition], sqlCommand);
                else // If we're scrubbing the whole field
                    columnValues[columnPosition] = ApplyActionsToWholeField(action, columnValues[columnPosition]);
            }
        }

        private string ApplyActionsToArray(ScrubfuTag action, string columnValue, string sqlCommand)
        {
            string[] arrayValues;
            if (columnValue.StartsWith("'{", StringComparison.Ordinal) && columnValue.EndsWith("}'", StringComparison.Ordinal))
                arrayValues = columnValue.Substring(2, columnValue.Length - 4).Split(",");
            else if (columnValue.StartsWith("{", StringComparison.Ordinal) && columnValue.EndsWith("}", StringComparison.Ordinal))
                arrayValues = columnValue.Substring(1, columnValue.Length - 2).Split(",");
            else
                return columnValue;

            // Process each array item, or just the specified one)
            int lowerBound = 0;
            int upperBound = arrayValues.Length - 1;

            if (action.Tag.ColumnArrayIndex.HasValue && action.Tag.ColumnArrayIndex.Value > 0)
                lowerBound = upperBound = action.Tag.ColumnArrayIndex.Value;

            for (int i = lowerBound; i <= upperBound; i++)
                arrayValues[i] = ApplyActionsToSpecificArrayElement(action, arrayValues[i]);

            if (sqlCommand == Constants.SqlCommand_INSERT_INTO)     // Insert into's require single quotes
                return string.Concat(new string[] { "'{", string.Join(",", arrayValues), "}'" });
            else
                return string.Concat(new string[] { "{", string.Join(",", arrayValues), "}" });
        }

        private string ApplyActionsToSpecificArrayElement(ScrubfuTag action, string arrayItemValue)
        {

            if (action.Tag is RandomTag)
            {
                if (arrayItemValue.IsWrappedByString("\""))
                    arrayItemValue = ((RandomTagOptions)action.Tag.Options).Pattern.WrapWithString("\"");
                else
                    arrayItemValue = ((RandomTagOptions)action.Tag.Options).Pattern;
            }

            if (arrayItemValue.IsWrappedByString("\""))
            {
                arrayItemValue = arrayItemValue.Substring(1, arrayItemValue.Length - 2);
                return action.Tag.Apply(arrayItemValue, action.Tag.GetOptions()).WrapWithString("\"");
            }
            else
               return action.Tag.Apply(arrayItemValue, action.Tag.GetOptions());
        }

        private string ApplyActionsToWholeField(ScrubfuTag action, string columnValue)
        {
            if (action.Tag is RandomTag)
            {
                if (columnValue.IsWrappedByString("'"))
                    columnValue = ((RandomTagOptions)action.Tag.Options).Pattern.WrapWithString("'");
                else
                    columnValue = ((RandomTagOptions)action.Tag.Options).Pattern;
            }

            return action.Tag.Apply(columnValue, action.Tag.GetOptions());
        }
        #endregion



        #region "SQL Parsing Methods"
        private SqlCreateTableDefinition ParseSqlCreateTableStatement(string sqlText)
        {
            if (!sqlText.StartsWith(Constants.SqlCommand_CREATE_TABLE, StringComparison.Ordinal))
                throw new SqlParseErrorException("table statement");

            var substringStart = sqlText.IndexOf(Constants.SqlCommand_CREATE_TABLE, StringComparison.Ordinal) +
                                 Constants.SqlCommand_CREATE_TABLE.Length;
            var substringEnd = sqlText.IndexOf("(", StringComparison.Ordinal);

            var entityName = sqlText.Substring(0, substringEnd).Substring(substringStart).Trim();

            substringStart = substringEnd + 1;
            substringEnd = sqlText.LastIndexOf(");", StringComparison.Ordinal);

            var tableColumnsText = sqlText.Substring(0, substringEnd).Substring(substringStart).Trim();

            var cols = tableColumnsText.Split(
                tableColumnsText.Contains(Environment.NewLine) ? Environment.NewLine : ",");

            for (var i = 0; i < cols.Length; i++)
                cols[i] = cols[i].TrimStart();

            var tableColumns = ParseSqlCreateTableStatementColumnDefinitions(cols);

            if (!string.IsNullOrWhiteSpace(entityName))
                return new SqlCreateTableDefinition
                {
                    Name = entityName.ToTableSchemaParts(),
                    Columns = tableColumns
                };

            return null;
        }

        private List<SqlColumnDefinition> ParseSqlCreateTableStatementColumnDefinitions(string[] columnDefinitions)
        {
            var columns = new List<SqlColumnDefinition>();

            foreach (var definition in columnDefinitions)
            {
                var remainder = definition.Trim();

                var name = remainder.Substring(0, remainder.IndexOf(" ", StringComparison.Ordinal));
                remainder = remainder.Substring(remainder.IndexOf(" ", StringComparison.Ordinal) + 1);

                var dataType = remainder;
                if (remainder.Contains(" "))
                    dataType = remainder.Substring(0, remainder.IndexOf(" ", StringComparison.Ordinal));

                var comment = "";
                if (remainder.Contains("--"))
                    comment = remainder
                        .Substring(remainder.IndexOf(Constants.FLAG_SQL_COMMENT_DELIMITER, StringComparison.Ordinal) +
                                   Constants.FLAG_SQL_COMMENT_DELIMITER.Length).Trim();

                columns.Add(new SqlColumnDefinition
                {
                    Name = name,
                    DataType = dataType,
                    Comment = comment
                });
            }

            return columns;
        }

        private SqlFieldDataType GetColumnDataType(SqlCreateTableDefinition tableDefinition, string columnName)
        {
            if (tableDefinition == null)
                throw new ArgumentNullException(nameof(tableDefinition));

            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentNullException(nameof(columnName));

            var columnDataType = tableDefinition.Columns.FirstOrDefault(c => c.Name.Equals(columnName))?.DataType;

            if (columnDataType == null)
                return SqlFieldDataType.Unknown;

            if (columnDataType.Contains("[") && columnDataType.Contains("]") || columnDataType.Contains("ARRAY"))
                return SqlFieldDataType.Array;

            return SqlFieldDataType.Unknown;
        }

        private string ParseEntityName(string sqlCommand, string sqlText)
        {
            if (string.IsNullOrEmpty(sqlCommand))
                throw new ArgumentNullException(sqlCommand);

            if (string.IsNullOrEmpty(sqlText))
                throw new ArgumentNullException(sqlText);

            if (sqlCommand == Constants.SqlCommand_INSERT_INTO || sqlCommand == Constants.SqlCommand_COPY)
            {
                var entityName = sqlText.SubstringFromUntil(new SubstringOptions
                {
                    From = new SubstringOption(sqlCommand + " "),
                    To = new SubstringOption("(")
                }).Trim();

                return entityName.Split(" ")[0];
            }

            throw new SqlEntityNameParseErrorException($"'{sqlCommand}' is not currently a supported command.");
        }

        private string[] ParseDefinedColumns(string sqlCommand, string sqlText)
        {
            if (string.IsNullOrEmpty(sqlCommand))
                throw new ArgumentNullException(sqlCommand);

            if (string.IsNullOrEmpty(sqlText))
                throw new ArgumentNullException(sqlText);

            if (sqlCommand == Constants.SqlCommand_INSERT_INTO || sqlCommand == Constants.SqlCommand_COPY)
            {
                var columnsText =
                    sqlText.SubstringFromUntil(new SubstringOptions(new SubstringOption("("),
                        new SubstringOption(")")));
                columnsText = columnsText.Replace(", ", ",");

                return columnsText.SplitWithValues(",");
            }

            throw new SqlDefinedColumnsParseErrorException($"'{sqlCommand}' is not currently a supported command.");
        }

        private bool CheckValidInsertIntoStatement(string text)
        {
            var matchFull = Regex.Match(text, Constants.SqlCommand_INSERT_INTO_FULL_REGEX, RegexOptions.IgnoreCase);
            if (matchFull.Success)
                return matchFull.Success;
            else
            {
                var matchPartial = Regex.Match(text, Constants.SqlCommand_INSERT_INTO_PARTIAL_REGEX, RegexOptions.IgnoreCase);
                return matchPartial.Success;
            }
        }

        private string[] ParseInsertStatementValues(string sqlText)
        {
            if (string.IsNullOrEmpty(sqlText))
                throw new ArgumentNullException(sqlText);

            string insertPreValuesPart = string.Empty;
            string preValuesPart = string.Empty;
            string valuesPart = string.Empty;
            string postValuesPart = string.Empty;
            SplitInsertValuesParts(sqlText, ref insertPreValuesPart, ref preValuesPart, ref valuesPart, ref postValuesPart);

            var values = new List<string>();
            var processText = valuesPart;

            while (!string.IsNullOrEmpty(processText))
                GetColumnValue(ref processText, ref values);

            return values.ToArray();
        }

        private void SplitInsertValuesParts(string sqlText, ref string insertPreValuesPart, ref string valuesPrePart, ref string valuesMainPart, ref string valuesPosPart)
        {
            string valuesStatementPart = Regex.Match(sqlText, Constants.SqlCommand_INSERT_VALUES_REGEX, RegexOptions.IgnoreCase).Value;

            if (valuesStatementPart == null)
                throw new SqlParseErrorException(sqlText);

            insertPreValuesPart = sqlText.Substring(0, sqlText.Length - valuesStatementPart.Length);
            valuesPrePart = valuesStatementPart.Substring(0, valuesStatementPart.IndexOf("(", StringComparison.Ordinal) + 1);

            int valuesStartPos = valuesStatementPart.IndexOf("(", StringComparison.Ordinal) + 1;
            int valuesEndPos = valuesStatementPart.LastIndexOf(")", StringComparison.Ordinal);
            int cutLength = valuesStatementPart.Length - valuesStartPos - (valuesStatementPart.Length - valuesEndPos);
            valuesMainPart = valuesStatementPart.Substring(valuesStartPos, cutLength);

            valuesPosPart = valuesStatementPart.Substring(valuesPrePart.Length + valuesMainPart.Length);
        }

        private string ReplaceInsertStatementValues(string sqlText, string[] columnValues)
        {
            if (string.IsNullOrEmpty(sqlText))
                throw new ArgumentNullException(sqlText);

            string insertPreValuesPart = string.Empty;
            string valuesPrePart = string.Empty;
            string valuesMainPart = string.Empty;
            string valuesPosPart = string.Empty;
            SplitInsertValuesParts(sqlText, ref insertPreValuesPart, ref valuesPrePart, ref valuesMainPart, ref valuesPosPart);

            var returnSB = new StringBuilder();
            returnSB.Append(insertPreValuesPart)
                .Append(valuesPrePart)
                .AppendJoin(", ", columnValues)
                .Append(valuesPosPart);

            return returnSB.ToString();
        }

        private void GetColumnValue(ref string processText, ref List<string> values)
        {
            const string comma = ",";

            var firstCommaIndex = processText.NthIndexOf(comma, 1);

            if (firstCommaIndex < 0)
            {
                values.Add(processText.Trim());
                processText = string.Empty;
                return;
            }

            var value = processText.Substring(0, firstCommaIndex);
            var remainder = processText.Substring(firstCommaIndex);

            const string textFieldTerminator = "'";
            if (!value.StartsWith(textFieldTerminator, StringComparison.Ordinal) ||
                value.EndsWith(textFieldTerminator, StringComparison.Ordinal))
            {
                values.Add(value.Trim());
                processText = processText.Substring(firstCommaIndex + comma.Length).Trim();
                return;
            }

            var terminatorFound = false;

            var terminatesAt = remainder.IndexOf(textFieldTerminator, StringComparison.Ordinal);

            var nextOccurence = 2;
            while (!terminatorFound)
                if (remainder.NthIndexOf(textFieldTerminator, nextOccurence) == terminatesAt + 1)
                {
                    terminatesAt = remainder.NthIndexOf(textFieldTerminator, nextOccurence + 1);

                    if (terminatesAt < 0)
                        throw new MalformedSqlParseErrorException("Malformed SQL detected.");

                    nextOccurence += 2;
                }
                else
                {
                    terminatorFound = true;
                }

            if (comma.ComesBefore(textFieldTerminator, remainder))
            {
                value += remainder.Substring(0, terminatesAt + textFieldTerminator.Length);

                remainder = remainder.Substring(terminatesAt + textFieldTerminator.Length).Trim();

                if (remainder.StartsWith(comma, StringComparison.Ordinal))
                    remainder = remainder
                        .Substring(remainder.IndexOf(comma, StringComparison.Ordinal) + comma.Length).Trim();
            }

            processText = remainder;

            values.Add(value.Trim());
        }
        #endregion
    }
}
