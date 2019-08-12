/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Scrubfu.Tests.CLI_Tests
{
    [Collection("CLI Test Collection")]
    public class ScrubfuCli_Fuzz_Tag_Tests
    {
        private readonly Dictionary<string,string> scrubDetails = new Dictionary<string, string>() { { "public.employees.first_name", "~FZ~" } };

        [Fact]
        public void Fuzz_Tag_With_Copy()
        {
            string randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string inputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-in.sql");
            string outputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-out.sql");

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);

            TestHelpers.GenerateSamplePGDumpFile(inputFilePath, UseCopy: true, scrubComments: scrubDetails);
            int inputLineCount = File.ReadAllLines(inputFilePath).Length;

            new ScrubfuCli().Run(TestHelpers.BuildCommandArgs(inputFilePath, outputFilePath));

            Assert.True(File.Exists(outputFilePath));

            string[] lines = File.ReadAllLines(outputFilePath);
            Assert.True(lines.Length == inputLineCount);

            var i = 0;
            foreach (var line in lines)
            {
                i++;

                if (i == Constants.COPY_TEST_SAMPLE_TEST_LINE_NUMBER)
                {
                    var testValue = line.Substring(10, 5);

                    if (!(char.IsLetter(testValue[0]) && char.IsUpper(testValue[0])))
                        Assert.False(true);

                    if (!char.IsLetter(testValue[1]))
                        Assert.False(true);

                    if (!char.IsLetter(testValue[2]))
                        Assert.False(true);

                    if (!char.IsLetter(testValue[3]))
                        Assert.False(true);

                    if (!char.IsLetter(testValue[4]))
                        Assert.False(true);
                }
            }

            Assert.True(true);

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);
        }

        [Fact]
        public void Fuzz_Tag_With_Inserts()
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

            string[] lines = File.ReadAllLines(outputFilePath);
            Assert.True(lines.Length == inputLineCount);

            var i = 0;
            foreach (var line in lines)
            {
                i++;

                if (i == Constants.INSERT_TEST_SAMPLE_TEST_LINE_NUMBER)
                {
                    var testValue = line.Substring(251, 5);

                    if (!(char.IsLetter(testValue[0]) && char.IsUpper(testValue[0])))
                        Assert.False(true);

                    if (!char.IsLetter(testValue[1]))
                        Assert.False(true);

                    if (!char.IsLetter(testValue[2]))
                        Assert.False(true);

                    if (!char.IsLetter(testValue[3]))
                        Assert.False(true);

                    if (!char.IsLetter(testValue[4]))
                        Assert.False(true);
                }
            }

            Assert.True(true);

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);
        }
    }
}