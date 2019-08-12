/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using Scrubfu.Exceptions;
using Scrubfu.Tags;
using Xunit;

namespace Scrubfu.Tests.Tag_Tests
{
    public class ReplaceTag_Tests
    {
        [Fact]
        public void Apply_EmptyString_EmptyReturned()
        {
            var TagOptions = string.Empty;
            var text = string.Empty;

            var replaceTag = new ReplaceTag(TagOptions);
            string result = replaceTag.Apply(text);

            Assert.True((result != null && result.Length == 0), "Empty input text should return empty output text");
        }

        [Theory]
        [InlineData(typeof(MaskTagOptions))]
        [InlineData(typeof(FuzzTagOptions))]
        [InlineData(typeof(RandomTagOptions))]
        public void Apply_NonReplaceOption_ExceptionReturned(Type type)
        {
            var TagOptions = string.Empty;
            var text = "mock text";
            var returnException = new Exception();

            dynamic nonReplaceOption = Activator.CreateInstance(type);
            var replaceTag = new ReplaceTag(TagOptions);
            try
            {
                replaceTag.Apply(text, nonReplaceOption);
            }
            catch (Exception ex)
            {
                returnException = ex;
            }

            Assert.True((returnException.GetType().Equals(new NoOptionsSuppliedException().GetType())), "Exception should be thrown if a non ReplaceOption is supplied.");
        }

        [Fact]
        public void Apply_SetOfPlainTuples_ReturnsReplacedString()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "Seattle,Cape Town;Tacoma,Johannesburg;Kirkland,Durban";
            var text = "I had a trip to Seattle, but wish it was New York. Any time I visit Tacoma or Kirkland, I get diverted to Egypt.";

            var replaceTag = new ReplaceTag(TagOptions);

            string result = replaceTag.Apply(text);

            Assert.True(result.Equals("I had a trip to Cape Town, but wish it was New York. Any time I visit Johannesburg or Durban, I get diverted to Egypt.")
                        , "Replace logic should work according to the mask tag rules");
        }
    }
}
