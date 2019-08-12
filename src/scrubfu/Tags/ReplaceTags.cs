/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
using System.Text.RegularExpressions;
using Scrubfu.Exceptions;
using Scrubfu.Extensions;
using Scrubfu.Contracts;

namespace Scrubfu.Tags
{
    public sealed class ReplaceTag : ParsedScrubfuTag
    {
        public ReplaceTag(string optionsText, int? columnArrayIndex = null)
        {
            priority = 2;
            OptionsText = optionsText;
            ColumnArrayIndex = columnArrayIndex;
        }

        public override IScrubfuTagOptions GetOptions(string optionsText = null)
        {
            var options = new ReplaceTagOptions();

            if (string.IsNullOrEmpty(optionsText))
                optionsText = OptionsText;

            if (string.IsNullOrEmpty(optionsText))
                return options;

            var replacements = optionsText.SplitWithValues(';', '\\');

            foreach (var replacement in replacements)
            {
                if (string.IsNullOrEmpty(replacement))
                    continue;

                var keyPair = replacement.SplitWithEscapeChar(',', '\\');

                if (keyPair.Length != 2)
                    throw new InvalidScrubfuTagException();

                options.Replacements.Add(keyPair[0].UnescapeAndRemoveSurroundingQuotes(), keyPair[1].UnescapeAndRemoveSurroundingQuotes());
          }

            return options;
        }

        public override string Apply(string fieldText, IScrubfuTagOptions commandOptions = null)
        {
            if (fieldText.Length == 0)
                return fieldText;

            var options = (commandOptions ?? Options) as ReplaceTagOptions;

            if (options == null)
                throw new NoOptionsSuppliedException(nameof(options));

            var isInQuotes = fieldText.IsWrappedByString(Constants.SINGLE_QUOTE);

            var offsetStart = isInQuotes ? 1 : 0;
            var offsetEnd = isInQuotes ? 1 : 0;

            var textToReplace = fieldText.Substring(offsetStart, fieldText.Length - (offsetStart + offsetEnd));

            if (string.IsNullOrEmpty(textToReplace))
                return fieldText;

            foreach (var replacement in options.Replacements)
                textToReplace = Regex.Replace(textToReplace, replacement.Key, replacement.Value, RegexOptions.None);

            if (isInQuotes)
                return textToReplace.WrapWithString(Constants.SINGLE_QUOTE);

            return textToReplace;
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