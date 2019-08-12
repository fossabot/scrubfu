/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System.Text;
using Scrubfu.Exceptions;
using Scrubfu.Extensions;
using Scrubfu.Services;
using Scrubfu.Contracts;

namespace Scrubfu.Tags
{
    public sealed class FuzzTag : ParsedScrubfuTag
    {
        private const string AlphaChars = "abcdefghijklmnopqrstuvwxyz";
        private const string CapitalAlphaChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string DigitChars = "1234567890";

        private readonly SafeRandomizer randomizer;

        public FuzzTag(string optionsText, int? columnArrayIndex = null)
        {
            priority = int.MaxValue;
            OptionsText = optionsText;
            ColumnArrayIndex = columnArrayIndex;

            randomizer = new SafeRandomizer();
        }

        public override IScrubfuTagOptions GetOptions(string optionsText = null)
        {
            return new FuzzTagOptions();
        }

        public override string Apply(string fieldText, IScrubfuTagOptions commandOptions = null)
        {
            if (fieldText.Length == 0)
                return fieldText;

            var options = (commandOptions ?? Options) as FuzzTagOptions;

            if (options == null)
                throw new NoOptionsSuppliedException(nameof(options));

            var isInQuotes = fieldText.IsWrappedByString(Constants.SINGLE_QUOTE);

            var offsetStart = isInQuotes ? 1 : 0;
            var offsetEnd = isInQuotes ? 1 : 0;

            var textToReplace = fieldText.Substring(offsetStart, fieldText.Length - (offsetStart + offsetEnd));

            if (string.IsNullOrEmpty(textToReplace))
                return fieldText;

            var sb = new StringBuilder();
            foreach (var character in textToReplace)
                sb.Append(GetFuzzCharacter(character));

            if (isInQuotes)
                return sb.ToString().WrapWithString(Constants.SINGLE_QUOTE);

            return sb.ToString();
        }

        private char GetFuzzCharacter(char character)
        {
            if (!char.IsLetterOrDigit(character))
                return character;

            if (char.IsDigit(character))
                return GetRandomDigit();

            return char.IsUpper(character) ? GetRandomCapitalAlphaCharacter() : GetRandomAlphaCharacter();
        }

        private char GetRandomDigit()
        {
            return DigitChars[randomizer.Next(0, DigitChars.Length - 1)];
        }

        private char GetRandomAlphaCharacter()
        {
            return AlphaChars[randomizer.Next(0, AlphaChars.Length - 1)];
        }

        private char GetRandomCapitalAlphaCharacter()
        {
            return CapitalAlphaChars[randomizer.Next(0, CapitalAlphaChars.Length - 1)];
        }

        public override bool ValidateTag()
        {
            return Options.ValidateOptions();
        }

    }
}