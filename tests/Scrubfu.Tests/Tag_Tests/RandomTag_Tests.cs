/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.Text.RegularExpressions;
using Scrubfu.Exceptions;
using Scrubfu.Tags;
using Xunit;

namespace Scrubfu.Tests.Tag_Tests
{
    public class RandomTag_Tests
    {
        [Fact]
        public void Apply_EmptyString_EmptyReturned()
        {
            var TagOptions = string.Empty;
            var text = string.Empty;

            var randomTag = new RandomTag(TagOptions);
            string result = randomTag.Apply(text);

            Assert.True((result != null && result.Length == 0), "Empty input text should return empty output text");
        }

        [Theory]
        [InlineData(typeof(MaskTagOptions))]
        [InlineData(typeof(FuzzTagOptions))]
        [InlineData(typeof(ReplaceTagOptions))]
        public void Apply_NonRandomOption_ExceptionReturned(Type type)
        {
            var TagOptions = string.Empty;
            var text = "mock text";
            var returnException = new Exception();

            dynamic nonRandomOption = Activator.CreateInstance(type);
            var randomTag = new RandomTag(TagOptions);
            try
            {
                randomTag.Apply(text, nonRandomOption);
            }
            catch (Exception ex)
            {
                returnException = ex;
            }

            Assert.True((returnException.GetType().Equals(new NoOptionsSuppliedException().GetType())), "Exception should be thrown if a non RandomOption is supplied.");
        }

        [Fact]
        public void Apply_Numerics_ReturnsRandomizedNumber()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "";
            var text = "+NNN(NNN) NNN-NNNN";

            var randomTag = new RandomTag(TagOptions);

            string result = randomTag.Apply(text);

            Match match = Regex.Match(result, @"\+[0-9]{3}\([0-9]{3}\) [0-9]{3}-[0-9]{4}", RegexOptions.Singleline);
            Assert.True(match.Success, "Fuzz logic should work according to the random tag rules");
        }

        [Fact]
        public void Apply_CapitalAlphas_ReturnsRandomizedNumber()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "";
            var text = "AAAA.AA.A.AAAA!";

            var randomTag = new RandomTag(TagOptions);

            string result = randomTag.Apply(text);

            Match match = Regex.Match(result, @"[A-Z]{4}\.[A-Z]{2}\.[A-Z]\.[A-Z]{4}!", RegexOptions.Singleline);
            Assert.True(match.Success, "Fuzz logic should work according to the random tag rules");
        }

        [Fact]
        public void Apply_LowerAlphas_ReturnsRandomizedNumber()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "";
            var text = "aaaa.aa.a.aaaa!";

            var randomTag = new RandomTag(TagOptions);

            string result = randomTag.Apply(text);

            Match match = Regex.Match(result, @"[a-z]{4}\.[a-z]{2}\.[a-z]\.[a-z]{4}!", RegexOptions.Singleline);
            Assert.True(match.Success, "Fuzz logic should work according to the random tag rules");
        }

        [Fact]
        public void Apply_Mix_ReturnsRandomizedNumber()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "";
            var text = "aaaa.AA.a.NNaa!";

            var randomTag = new RandomTag(TagOptions);

            string result = randomTag.Apply(text);

            Match match = Regex.Match(result, @"[a-z]{4}\.[A-Z]{2}\.[a-z]\.[0-9]{2}[a-z]{2}!", RegexOptions.Singleline);
            Assert.True(match.Success, "Fuzz logic should work according to the random tag rules");
        }
    }
}

