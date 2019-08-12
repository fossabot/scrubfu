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
    public sealed class RandomTag : ParsedScrubfuTag
    {
        private const string AlphaChars = "abcdefghijklmnopqrstuvwxyz";
        private const string CapitalAlphaChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string DigitChars = "1234567890";

        private readonly SafeRandomizer randomizer;

        public RandomTag(string optionsText, int? columnArrayIndex = null)
        {
            priority = 3;
            OptionsText = optionsText;
            ColumnArrayIndex = columnArrayIndex;

            randomizer = new SafeRandomizer();
        }

        public override IScrubfuTagOptions GetOptions(string optionsText = null)
        {
            var options = new RandomTagOptions();

            if (string.IsNullOrEmpty(optionsText))
                optionsText = OptionsText;

            if (string.IsNullOrEmpty(optionsText))
                return options;

            options.Pattern = optionsText;

            return options;
        }
        
        public override string Apply(string fieldText, IScrubfuTagOptions commandOptions = null)
        {
            if (fieldText.Length < 1)
                return fieldText;

            var options = (commandOptions ?? Options) as RandomTagOptions;

            if (options == null)
                throw new NoOptionsSuppliedException(nameof(options));

            var isInQuotes = fieldText.IsWrappedByString(Constants.SINGLE_QUOTE);

            var offsetStart = isInQuotes ? 1 : 0;
            var offsetEnd = isInQuotes ? 1 : 0;

            var textToRandomize = fieldText.Substring(offsetStart, fieldText.Length - (offsetStart + offsetEnd));

            var sb = new StringBuilder();
            foreach (var character in textToRandomize)
                sb.Append(GetRandomCharacter(character));

            return isInQuotes ? sb.ToString().WrapWithString(Constants.SINGLE_QUOTE) : sb.ToString();
        }

        private char GetRandomCharacter(char charType)
        {
            if (charType == 'N')
                return GetRandomDigit();

            if (charType == 'A')
                return GetRandomCapitalAlphaCharacter();

            if (charType == 'a')
                return GetRandomAlphaCharacter();

            return charType;
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