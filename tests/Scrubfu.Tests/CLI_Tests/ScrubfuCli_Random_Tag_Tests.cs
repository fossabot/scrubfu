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
    public class ScrubfuCli_Random_Tag_Tests
    {
        private readonly Dictionary<string, string> scrubDetails = new Dictionary<string, string>() { { "public.employees.city", "--~RE:Seattle,Cape Town;Tacoma,Johannesburg;Kirkland,Durban~" } };

        [Fact]
        public void Random_Tag_With_Copy()
        {
            string randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string inputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-in.sql");
            string outputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-out.sql");
            string logFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-log.log");

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);

            TestHelpers.GenerateSamplePGDumpFile(inputFilePath, UseCopy: false, scrubComments: scrubDetails);
            int inputLineCount = File.ReadAllLines(inputFilePath).Length;

            new ScrubfuCli().Run(TestHelpers.BuildCommandArgs(inputFilePath, outputFilePath, logFilePath));

            Assert.True(File.Exists(outputFilePath));

            var lines = File.ReadAllLines(outputFilePath);
            Assert.True(lines.Length == inputLineCount);

            var i = 0;
            foreach (var line in lines)
            {
                i++;

                if (i == Constants.COPY_TEST_SAMPLE_TEST_LINE_NUMBER)
                {
                    var testValue = line.Substring(113, 18);

                    if (testValue[0] != '+')
                        Assert.False(true);

                    if (!char.IsDigit(testValue[1]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[2]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[3]))
                        Assert.False(true);

                    if (testValue[4] != '(')
                        Assert.False(true);

                    if (!char.IsDigit(testValue[5]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[6]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[7]))
                        Assert.False(true);

                    if (testValue[8] != ')')
                        Assert.False(true);

                    if (testValue[9] != ' ')
                        Assert.False(true);

                    if (!char.IsDigit(testValue[10]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[11]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[12]))
                        Assert.False(true);

                    if (testValue[13] != '-')
                        Assert.False(true);

                    if (!char.IsDigit(testValue[14]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[15]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[16]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[17]))
                        Assert.False(true);
                }
            }

            Assert.True(true);

            //File.Delete(inputFilePath);
            //File.Delete(outputFilePath);
            //File.Delete(logFilePath);
        }

        [Fact]
        public void Random_Tag_With_Inserts()
        {
            string randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string inputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-in.sql");
            string outputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-out.sql");
            string logFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-log.log");

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);

            TestHelpers.GenerateSamplePGDumpFile(inputFilePath, UseCopy: false, scrubComments: scrubDetails);
            int inputLineCount = File.ReadAllLines(inputFilePath).Length;

            new ScrubfuCli().Run(TestHelpers.BuildCommandArgs(inputFilePath, outputFilePath, logFilePath));

            Assert.True(File.Exists(outputFilePath));

            var lines = File.ReadAllLines(outputFilePath);
            Assert.True(lines.Length == inputLineCount);


            var i = 0;
            foreach (var line in lines)
            {
                i++;

                if (i == Constants.INSERT_TEST_SAMPLE_TEST_LINE_NUMBER)
                {
                    var testValue = line.Substring(383, 18);

                    if (testValue[0] != '+')
                        Assert.False(true);

                    if (!char.IsDigit(testValue[1]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[2]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[3]))
                        Assert.False(true);

                    if (testValue[4] != '(')
                        Assert.False(true);

                    if (!char.IsDigit(testValue[5]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[6]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[7]))
                        Assert.False(true);

                    if (testValue[8] != ')')
                        Assert.False(true);

                    if (testValue[9] != ' ')
                        Assert.False(true);

                    if (!char.IsDigit(testValue[10]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[11]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[12]))
                        Assert.False(true);

                    if (testValue[13] != '-')
                        Assert.False(true);

                    if (!char.IsDigit(testValue[14]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[15]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[16]))
                        Assert.False(true);

                    if (!char.IsDigit(testValue[17]))
                        Assert.False(true);
                }
            }

            Assert.True(true);

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);
            File.Delete(logFilePath);
        }
    }
}