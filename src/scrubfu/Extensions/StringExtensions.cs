/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Scrubfu.Models;

namespace Scrubfu.Extensions
{
    public static class StringExtensions
    {
        public static int NthIndexOf(this string text, string match, int occurence)
        {
            var i = 1;
            var index = -1;

            while (i < occurence + 1 && (index = text.IndexOf(match, index + 1, StringComparison.Ordinal)) != -1)
            {
                if (i == occurence)
                    return index;

                i++;
            }

            return -1;
        }

        public static SchemaNameParts ToColumnSchemaParts(this string text)
        {
            var parts = text.Split(".");

            return new SchemaNameParts
            {
                Schema = parts[0],
                Table = parts[1],
                Column = parts[2]
            };
        }

        public static SchemaNameParts ToTableSchemaParts(this string text)
        {
            var parts = text.Split(".");

            return new SchemaNameParts
            {
                Schema = parts[0],
                Table = parts[1]
            };
        }

        public static bool IsWrappedByString(this string text, string wrapString)
        {
            return text.StartsWith(wrapString, StringComparison.Ordinal) && text.EndsWith(wrapString, StringComparison.Ordinal);
        }

        public static bool IsEscaped(this string text)
        {
            return (text.StartsWith("\\") && text.Length > 1);
        }

        public static string[] SplitWithEscapeChar(this string value, char delimiterChar, char escapeChar)
        {
            var result = new List<string>();
        
            int segmentStart = 0;
            for (int i = 0; i < value.Length; i++)
            {
                bool readEscapeChar = false;
                if (value[i] == escapeChar)
                {
                    readEscapeChar = true;
                    i++;
                }
            
                if (!readEscapeChar && value[i] == delimiterChar)
                {
                    result.Add(UnEscapeString( value.Substring(segmentStart, i - segmentStart), delimiterChar, escapeChar));
                    segmentStart = i + 1;
                }
            
                if (i == value.Length - 1)
                    result.Add(UnEscapeString(value.Substring(segmentStart), delimiterChar, escapeChar));
            }
        
            return result.ToArray();
        }

        private static string UnEscapeString(string src, char delimiterChar, char escapeChar)
        {
            return src.Replace(string.Concat(escapeChar, delimiterChar), delimiterChar.ToString())
            .Replace(string.Concat(escapeChar, escapeChar), delimiterChar.ToString());
        }
        
        public static string WrapWithString(this string text, string wrapString)
        {
            return string.Format("{0}{1}{0}", wrapString, text);
        }

        public static string[] SplitWithValues(this string text, string splitString)
        {
            return text.IndexOf(splitString, StringComparison.Ordinal) > -1 ? text.Split(splitString) : new[] {text};
        }

        public static string[] SplitWithValues(this string text, char splitChar, char escapeChar)
        {
            return text.IndexOf(splitChar, StringComparison.Ordinal) > -1 ? text.SplitWithEscapeChar(splitChar, escapeChar) : new[] {text};
        }

        public static string SubstringFromUntil(this string text, SubstringOptions options)
        {
            if (options == null)
                throw new ArgumentException(nameof(options));

            if (string.IsNullOrEmpty(options.From?.Match) && string.IsNullOrEmpty(options.To?.Match))
                return null;

            var value = text;

            if (options.From != null && !string.IsNullOrEmpty(options.From?.Match))
            {
                var startPosition = value.NthIndexOf(options.From.Match, options.From.Occurrence);

                if (startPosition < 0)
                    throw new InvalidOperationException($"'{options.From?.Match}' could not be found in the string.");

                value = value.Substring(startPosition + (options.IncludeFromInSubstring ? 0 : options.From.Match.Length));
            }

            if (options.To != null && !string.IsNullOrEmpty(options.To.Match))
            {
                var endPosition = value.NthIndexOf(options.To.Match, options.To.Occurrence);

                if (endPosition < 0)
                    throw new InvalidOperationException($"'{options.To.Match}' could not be found in the string.");

                value = value.Substring(0, endPosition);
            }

            return value;
        }

        public static bool ComesBefore(this string str1, string str2, string text)
        {
            var index1 = text.IndexOf(str1, StringComparison.Ordinal);
            var index2 = text.IndexOf(str2, StringComparison.Ordinal);

            return index1 < index2;
        }

        public static string RemoveSurroundingQuotes(this string value)
        {
            for (int i = 0; i < 2; i++) // Repeat this twice in case 2 single quotes are used as escape
            {
                if (((value[0] == '\'' && value[value.Length - 1] == '\'') || (value[0] == '"' && value[value.Length - 1] == '"')) && value.Length >= 2)
                    value = value.Substring(1, value.Length - 2);
            }
            
            return value;
        }

        public static string UnescapeAndRemoveSurroundingQuotes(this string value)
        {
            return RemoveSurroundingQuotes(Regex.Unescape(value));
        }
    }

    public class SubstringOptions
    {
        public SubstringOptions()
        {
        }

        public SubstringOptions(SubstringOption from, SubstringOption to)
        {
            From = from;
            To = to;
        }

        public SubstringOption From { get; set; }

        public SubstringOption To { get; set; }

        public bool IncludeFromInSubstring { get; set; } = false;
    }

    public class SubstringOption
    {
        public SubstringOption(string match, int occurrence = 1)
        {
            Match = match;
            Occurrence = occurrence;
        }

        public string Match { get; set; }

        public int Occurrence { get; set; }
    }
}