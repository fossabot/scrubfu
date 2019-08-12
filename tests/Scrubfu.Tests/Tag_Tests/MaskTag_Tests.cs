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
using Scrubfu.Tests.Models;
using Xunit;

namespace Scrubfu.Tests.Tag_Tests
{
    public class MaskTag_Tests
    {
        [Fact]
        public void Apply_EmptyString_EmptyReturned()
        {
            var TagOptions = string.Empty;
            var text = string.Empty;

            var maskTag = new MaskTag(TagOptions);
            string result = maskTag.Apply(text);

            Assert.True((result != null && result.Length == 0), "Empty input text should return empty output text");
        }

        [Theory]
        [InlineData(typeof(FuzzTagOptions))]
        [InlineData(typeof(RandomTagOptions))]
        [InlineData(typeof(ReplaceTagOptions))]
        public void Apply_NonMaskOption_ExceptionReturned(Type type)
        {
            var TagOptions = string.Empty;
            var text = "mock text";
            var returnException = new Exception();

            dynamic nonFuzzOption = Activator.CreateInstance(type);
            var maskTag = new MaskTag(TagOptions);
            try
            {
                maskTag.Apply(text, nonFuzzOption);
            }
            catch (Exception ex)
            {
                returnException = ex;
            }

            Assert.True((returnException.GetType().Equals(new NoOptionsSuppliedException().GetType())), "");
        }

        [Fact]
        public void Apply_EmailStringWithSingleQuotesOnStrings_ReturnsMaskedEmailString1()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "3,2;#;'@','.'";
            var text = "peter.smith@gmail.com";

            var maskTag = new MaskTag(TagOptions);

            string result = maskTag.Apply(text);

            Match match = Regex.Match(result, "[a-z]{3}#{2}.#{5}@#{5}.#om", RegexOptions.Singleline);
            Assert.True(match.Success, "Mask logic should work according to the mask tag rules");
        }

        [Fact]
        public void Apply_EmailStringWithSingleQuotesOnStrings_ReturnsMaskedEmailString2()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "1,0;*;'@','.'";
            var text = "peter.smith@gmail.com";

            var maskTag = new MaskTag(TagOptions);

            string result = maskTag.Apply(text);

            Match match = Regex.Match(result, @"[a-z]{1}\*{4}.\*{5}@\*{5}.\*{3}", RegexOptions.Singleline);
            Assert.True(match.Success, "Mask logic should work according to the mask tag rules");
        }

        [Fact]
        public void Apply_EmailStringWithSingleQuotesOnStrings_ReturnsMaskedEmailString3()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "0,0;.;'@','.'";
            var text = "peter.smith@gmail.com";

            var maskTag = new MaskTag(TagOptions);

            string result = maskTag.Apply(text);

            Match match = Regex.Match(result, @"\.{11}@.{9}", RegexOptions.Singleline);
            Assert.True(match.Success, "Mask logic should work according to the mask Tag rules");
        }

        [Fact]
        public void Apply_EmailStringWithoutSingleQuotesOnStrings_ReturnsMaskedEmailString1()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "3,2;#;@,.";
            var text = "peter.smith@gmail.com";

            var maskTag = new MaskTag(TagOptions);

            string result = maskTag.Apply(text);

            Match match = Regex.Match(result, "[a-z]{3}#{2}.#{5}@#{5}.#om", RegexOptions.Singleline);
            Assert.True(match.Success, "Mask logic should work according to the mask tag rules");
        }

        [Fact]
        public void Apply_EmailStringWithoutSingleQuotesOnStrings_ReturnsMaskedEmailString2()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "1,0;*;@,.";
            var text = "peter.smith@gmail.com";

            var maskTag = new MaskTag(TagOptions);

            string result = maskTag.Apply(text);

            Match match = Regex.Match(result, @"[a-z]{1}\*{4}.\*{5}@\*{5}.\*{3}", RegexOptions.Singleline);
            Assert.True(match.Success, "Mask logic should work according to the mask tag rules");
        }

        [Fact]
        public void Apply_EmailStringWithoutSingleQuotesOnStrings_ReturnsMaskedEmailString3()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "0,0;.;@,.";
            var text = "peter.smith@gmail.com";

            var maskTag = new MaskTag(TagOptions);

            string result = maskTag.Apply(text);

            Match match = Regex.Match(result, @"\.{11}@.{9}", RegexOptions.Singleline);
            Assert.True(match.Success, "Mask logic should work according to the mask tag rules");
        }

        [Fact]
        public void Apply_EmailStringWithSingleQuotesOnBiggerStringsNoPreOrPost_ReturnsMaskedEmailString()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "0,0;#;'ma','.','com'";
            var text = "peter.smith@gmail.com";

            var maskTag = new MaskTag(TagOptions);

            string result = maskTag.Apply(text);

            Match match = Regex.Match(result, "#{5}.#{7}ma#{2}.com", RegexOptions.Singleline);
            Assert.True(match.Success, "Mask logic should work according to the mask tag rules");
        }

        [Fact]
        public void Apply_EmailStringWithSingleQuotesOnBiggerStringsWithPreOrPost_ReturnsMaskedEmailString()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "3,2;#;'ma','.','@','com'";
            var text = "peter.smith@gmail.com";

            var maskTag = new MaskTag(TagOptions);

            string result = maskTag.Apply(text);

            Match match = Regex.Match(result, "pet#{2}.#{5}.#ma#{2}.#om", RegexOptions.Singleline);
            Assert.True(match.Success, "Mask logic should work according to the mask tag rules");
        }

        [Fact]
        public void Apply_EmailStringWithSingleQuotesOnBiggerStringsWithPreOrPostAndSingleQuote_ReturnsMaskedEmailString()
        {
            // This test caters for mask rules in accordance with the test input
            var TagOptions = "3,2;#;'ma','.','com',''th','@'";
            var text = "peter.smi'th@gmail.com";

            var maskTag = new MaskTag(TagOptions);

            string result = maskTag.Apply(text);

            Match match = Regex.Match(result, "pet#{2}.#{3}'th@#ma#{2}.#om", RegexOptions.Singleline);
            Assert.True(match.Success, "Mask logic should work according to the mask tag rules");
        }
    }
}
