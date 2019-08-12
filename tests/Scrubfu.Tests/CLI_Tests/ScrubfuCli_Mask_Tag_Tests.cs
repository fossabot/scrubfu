/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Scrubfu.Tests.CLI_Tests
{
    [Collection("CLI Test Collection")]
    public class ScrubfuCli_Mask_Tag_Tests
    {
        private const string mask_tag_copy_sample_line12_result = @"1	Davolio	Nancy	Sales Representative	Ms.	1948-12-08	1992-05-01	507 - 20th Ave. E.\\nApt. 2A	Seattle	WA	98122	USA	(206) 555-9857	5467	\\x	Education includes a BA in psychology from Colorado State University in 1970.  She also completed The Art of the Cold Call.  Nancy is a member of Toastmasters International.	2	http://accweb/emmployees/davolio.bmp	nan##@####.#om";
        private const string mask_tag_inserts_sample_line11_result = @"INSERT INTO public.employees (employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path, email) VALUES (1, 'Davolio', 'Nancy', 'Sales Representative', 'Ms.', '1948-12-08', '1992-05-01', '507 - 20th Ave. E.\nApt. 2A', 'Seattle', 'WA', '98122', 'USA', '(206) 555-9857', '5467', '\x', 'Education includes a BA in psychology from Colorado State University in 1970.  She also completed The Art of the Cold Call.  Nancy is a member of Toastmasters International.', 2, 'http://accweb/emmployees/davolio.bmp', 'nan##@####.#om');";
        private readonly Dictionary<string, string> scrubDetails = new Dictionary<string, string>() { { "public.employees.email", "~MA:3,2;#;@,.~" } };

        [Fact]
        public void Mask_Tag_With_Copy()
        {
            string randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string inputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-in.sql");
            string outputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-out.sql");

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);

            TestHelpers.GenerateSamplePGDumpFile(inputFilePath, UseCopy: false, scrubComments: scrubDetails);
            int inputLineCount = File.ReadAllLines(inputFilePath).Length;

            new ScrubfuCli().Run(TestHelpers.BuildCommandArgs(inputFilePath, outputFilePath));

            Assert.True(File.Exists(outputFilePath));

            var lines = File.ReadAllLines(outputFilePath);
            Assert.True(lines.Length == inputLineCount);

            var i = 0;
            foreach (var line in lines)
            {
                i++;
                Assert.False(i == Constants.COPY_TEST_SAMPLE_TEST_LINE_NUMBER && !line.Equals(mask_tag_copy_sample_line12_result, StringComparison.Ordinal));
            }

            Assert.True(true);

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);
        }

        [Fact]
        public void Mask_Tag_With_Inserts()
        {
            string randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string inputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-in.sql");
            string outputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-out.sql");

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);

            TestHelpers.GenerateSamplePGDumpFile(inputFilePath, UseCopy: false, scrubComments: scrubDetails);
            int inputLineCount = File.ReadAllLines(inputFilePath).Length;

            new ScrubfuCli().Run(TestHelpers.BuildCommandArgs(inputFilePath, outputFilePath));

            Assert.True(File.Exists(outputFilePath));

            var lines = File.ReadAllLines(outputFilePath);
            Assert.True(lines.Length == inputLineCount);

            var i = 0;
            foreach (var line in lines)
            {
                i++;
                Assert.False(i == Constants.INSERT_TEST_SAMPLE_TEST_LINE_NUMBER && !line.Equals(mask_tag_inserts_sample_line11_result, StringComparison.Ordinal));
            }

            Assert.True(true);

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);
        }
    }
}