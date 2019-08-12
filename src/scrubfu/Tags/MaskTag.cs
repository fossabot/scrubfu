/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Scrubfu.Exceptions;
using Scrubfu.Extensions;
using Scrubfu.Contracts;

namespace Scrubfu.Tags
{
    public sealed class MaskTag : ParsedScrubfuTag
    {
        public MaskTag(string optionsText, int? columnArrayIndex = null)
        {
            priority = 1;
            OptionsText = optionsText;
            ColumnArrayIndex = columnArrayIndex;
        }

        public override IScrubfuTagOptions GetOptions(string optionsText = null)
        {
            var options = new MaskTagOptions();

            if (string.IsNullOrEmpty(optionsText))
                optionsText = OptionsText;

            if (string.IsNullOrEmpty(optionsText))
                return options;

            var optionParts = optionsText.SplitWithValues(";");

            var maskOffsets = optionParts[0].SplitWithValues(",");

            if (int.TryParse(maskOffsets[0], out int outStartVal))
                options.MaskStartOffset = outStartVal;

            if (int.TryParse(maskOffsets[1], out int outEndVal))
                options.MaskEndOffset = outEndVal;

            if (optionParts.Length < 2)
                return options;

            if (optionParts[1].IsWrappedByString("'"))
                options.MaskChar = char.Parse(optionParts[1].Substring(1, 1));
            else if (optionParts[1].IsEscaped())
                options.MaskChar = char.Parse(optionParts[1].Substring(1, 1));
            else
                options.MaskChar = char.Parse(optionParts[1].Substring(0,1));

            if (optionParts.Length < 3)
                return options;

            if (!string.IsNullOrEmpty(optionParts[2]))
                options.IgnoreStrings = optionParts[2].SplitWithValues(",");

            RemoveSingleQuotesAndUnescapeIgnoreStrings(ref options);

            return options;
        }

        private static void RemoveSingleQuotesAndUnescapeIgnoreStrings(ref MaskTagOptions options)
        {
            // Remove leading and trailing single quotes, but leave any single quotes that are meant to be part of the ignore string
            for (var i = 0; i < options.IgnoreStrings.Length; i++)
            {   
                options.IgnoreStrings[i] = options.IgnoreStrings[i].RemoveSurroundingQuotes();
                options.IgnoreStrings[i] = Regex.Unescape(options.IgnoreStrings[i]);
            }
        }

        private static bool ShouldMaskFullText(int textLength, int offsetStart, int maskCharTotal)
        {
            // If fieldText length is <= the starting mask offset.
            if (textLength < offsetStart + 1)
                return true;

            // If fieldText is shorter than the starting offset and the total characters.
            if (textLength < offsetStart + maskCharTotal)
                return true;

            return false;
        }

        private static int CalculateMaskCharTotal(int textLength, int offsetStart, int offsetEnd)
        {
            return textLength - (offsetStart + offsetEnd) > -1 ? textLength - (offsetStart + offsetEnd) : 0;
        }

        public override string Apply(string fieldText, IScrubfuTagOptions commandOptions = null)
        {
            if (fieldText.Length < 1)
                return fieldText;

            var options = (commandOptions ?? Options) as MaskTagOptions;

            if (options == null)
                throw new NoOptionsSuppliedException(nameof(options));

            var inQuotes = fieldText.IsWrappedByString("'");

            if (inQuotes)
                fieldText = fieldText.Substring(1, fieldText.Length - 2);

            var offsetStart = options.MaskStartOffset;
            var offsetEnd = options.MaskEndOffset;

            var maskCharTotal = CalculateMaskCharTotal(fieldText.Length, offsetStart, offsetEnd);

            if (ShouldMaskFullText(fieldText.Length, offsetStart, maskCharTotal))
            {
                offsetStart = 0;
                maskCharTotal = fieldText.Length;
            }

            var textToMask = fieldText.Substring(offsetStart, maskCharTotal);

            if (string.IsNullOrEmpty(textToMask))
                return inQuotes ? fieldText.WrapWithString("'") : fieldText;

            var sb = new StringBuilder();

            sb.Append(fieldText.Substring(0, offsetStart));

            var regExPatternSB = new StringBuilder().Append("(");
            foreach (var ignoreString in options.IgnoreStrings)
                regExPatternSB.Append(string.Concat(Regex.Escape(ignoreString), "|"));

            regExPatternSB.Append(".)");
            sb.Append(Regex.Replace(textToMask, regExPatternSB.ToString(), delegate (Match match)
            {
                return options.IgnoreStrings.Contains(match.Value) ? match.Value : options.MaskChar.ToString();
            }));

            sb.Append(fieldText.Substring(textToMask.Length + offsetStart));

            return inQuotes ? sb.ToString().WrapWithString("'") : sb.ToString();
        }

        public override bool ValidateTag()
        {
            try
            {
                return Options.ValidateOptions();
            }
            catch
            {
                return false;
            }
        }
    }
}