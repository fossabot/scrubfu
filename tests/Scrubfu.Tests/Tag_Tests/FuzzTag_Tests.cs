/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
using System;
using System.Text.RegularExpressions;
using Scrubfu.Exceptions;
using Scrubfu.Tags;
using Xunit;

namespace Scrubfu.Tests.Tag_Tests
{
    public class FuzzTag_Tests
    {
        [Fact]
        public void Apply_EmptyString_EmptyReturned()
        {
            var TagOptions = string.Empty;
            var text = string.Empty;

            var fuzzTag = new FuzzTag(TagOptions);
            string result = fuzzTag.Apply(text);

            Assert.True((result != null && result.Length == 0), "Empty input text should return empty output text");
        }

        [Theory]
        [InlineData(typeof(MaskTagOptions))]
        [InlineData(typeof(RandomTagOptions))]
        [InlineData(typeof(ReplaceTagOptions))]
        public void Apply_NonFuzzOption_ExceptionReturned(Type type)
        {
            var TagOptions = string.Empty;
            var text = "mock text";
            var returnException = new Exception();

            dynamic nonFuzzOption = Activator.CreateInstance(type);
            var fuzzTag = new FuzzTag(TagOptions);
            try
            {
                fuzzTag.Apply(text, nonFuzzOption);
            }
            catch (Exception ex)
            {
                returnException = ex;
            }

            Assert.True((returnException.GetType().Equals(new NoOptionsSuppliedException().GetType())), "Exception should be thrown if a non FuzzOption is supplied.");
        }

        [Fact]
        public void Apply_MixedString_ReturnsExactMixedString()
        {
            // This test caters for the following fuzz rules:
            //  - Uppercase letters to random uppercase letters
            //  - Lowercase letters to random lowercase letters
            //  - Digits to random digits
            //  - Other characters untouched

            var TagOptions = string.Empty;
            var text = "'/Ed1UcAt3Ion'/";

            var fuzzTag = new FuzzTag(TagOptions);

            string result = fuzzTag.Apply(text);

            Match match = Regex.Match(result, @"'\/[A-Z][a-z][0-9][A-Z][a-z][A-Z][a-z][0-9][A-Z][a-z][a-z]'\/", RegexOptions.Singleline);
            Assert.True(match.Success, "Fuzz logic should work according to the fuzz tag rules");
        }
    }
}
