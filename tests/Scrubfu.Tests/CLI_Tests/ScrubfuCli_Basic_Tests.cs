/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
using System;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace Scrubfu.Tests.CLI_Tests
{
    [Collection("CLI Test Collection")]
    public class ScrubfuCli_Basic_Tests
    {
        [Theory]
        [InlineData("--version")]
        [InlineData("-v")]
        [InlineData("--help")]
        [InlineData("-h")]
        [InlineData("")]
        public void Run_BasicOptionTests_CorrectItemsReturned(string inputString)
        {
            var args = TestHelpers.GetArgs(inputString);
            string result = string.Empty;
            var currentConsoleOut = Console.Out;

            try
            {
                using (var consoleOutput = new ConsoleOutput())
                {
                    new ScrubfuCli().Run(args);
                    result = consoleOutput.GetOuput();
                }
            }
            catch (Exception)
            {
                Assert.False(true);
            }

            // Confirm original console out has been restored.
            Assert.Equal(currentConsoleOut, Console.Out);

            switch (inputString)
            {
                case "--version":
                case "-v":
                    {
                        Match match = Regex.Match(result, @"[S|s]crubfu v[0-9]+.[0-9]+.[0-9]+", RegexOptions.Singleline);
                        Assert.True(match.Success, string.Format("{0} should return correct statement.", inputString));
                        break;
                    }
                case "--help":
                case "-h":
                case "":
                    {
                        Match matchShouldHave = Regex.Match(result, @"Usage: scrubfu \[OPTIONS\] \[INFILE\] \[OUTFILE\]", RegexOptions.Singleline);
                        Match matchShouldNotHave = Regex.Match(result, @"(Error|error|Invalid|invalid])b", RegexOptions.Singleline);
                        Assert.True((matchShouldHave.Success && !matchShouldNotHave.Success), string.Format("{0} should return correct statement.", inputString));
                        break;
                    }
                default:
                    {
                        Assert.True(false);  // Force fail for unhandled cases
                        break;
                    }
            }
        }

        [Theory]
        [InlineData("--some-incorrect-option")]
        [InlineData("--log")]   // Incomplete log arg
        [InlineData("--log_level")]   // Incomplete log_level arg
        public void Run_InvalidOptionTests_CorrectErrorsReturned(string inputString)
        {
            var args = TestHelpers.GetArgs(inputString);
            string result = string.Empty;
            var currentConsoleErrorOut = Console.Error;

            try
            {

                using (var consoleErrorOutput = new ConsoleErrorOutput())
                {
                    new ScrubfuCli().Run(args);
                    result = consoleErrorOutput.GetOuput();
                }
            }
            catch (Exception)
            {
                Assert.False(true);
            }

            // Confirm original console out has been restored.
            Assert.Equal(currentConsoleErrorOut, Console.Error);

            switch (inputString)
            {
                case "--some-incorrect-option":
                    {
                        Match matchShouldHave = Regex.Match(result, @"Run --help to get usage information", RegexOptions.Singleline);
                        Match matchShouldAlsoHave = Regex.Match(result, @"(Error|error|Invalid|invalid])b", RegexOptions.Singleline);
                        Assert.True((matchShouldHave.Success && matchShouldHave.Success), string.Format("{0} should return correct statement.", inputString));
                        break;
                    }
                case "--log":
                    {
                        Match matchShouldHave = Regex.Match(result, @"Error: Log file not a valid file location", RegexOptions.Singleline);
                        Assert.True((matchShouldHave.Success), string.Format("{0} should return correct statement.", inputString));
                        break;
                    }
                case "--log_level":
                    {
                        Match matchShouldHave = Regex.Match(result, @"Error: Log level specified is not valid. Valid options are", RegexOptions.Singleline);
                        Assert.True((matchShouldHave.Success), string.Format("{0} should return correct statement.", inputString));
                        break;
                    }
                default:
                    {
                        Assert.True(false);  // Force fail for unhandled cases
                        break;
                    }
            }
        }

        [Fact]
        public void Run_LogSet_LogFIleMoved()
        {
            string logFilePath = string.Concat(TestHelpers.GetOutputFilePath(), "testloglevel.log");
            string randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string inputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-in.sql");
            string outputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-out.sql");
            var args = TestHelpers.GetArgs($"{inputFilePath} {outputFilePath} --log {logFilePath}");

            File.Delete(logFilePath);
            File.Delete(inputFilePath);
            File.Delete(outputFilePath);

            TestHelpers.GenerateSamplePGDumpFile(inputFilePath, UseCopy: false);

            new ScrubfuCli().Run(args);

            var fileInfo = new FileInfo(logFilePath);
            Assert.True(fileInfo.Exists);
            Assert.True(fileInfo.Length > 0);

            File.Delete(outputFilePath);
            File.Delete(inputFilePath);
            File.Delete(logFilePath);
        }

        [Fact]
        public void Run_LogLevelSet_LogLevelLogged()
        {
            string logFilePath = string.Concat(TestHelpers.GetOutputFilePath(), "testloglevel.log");
            string randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string inputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-in.sql");
            string outputFilePath = string.Concat(TestHelpers.GetOutputFilePath(), randomFileName, "-out.sql");
            string[] args = TestHelpers.GetArgs($"{inputFilePath} {outputFilePath} --log {logFilePath} --log_level info");

            if (File.Exists(logFilePath))
                File.Delete(logFilePath);

            if (File.Exists(inputFilePath))
                File.Delete(inputFilePath);

            if (File.Exists(outputFilePath))
                File.Delete(outputFilePath);

            TestHelpers.GenerateSamplePGDumpFile(inputFilePath, UseCopy: false);

            new ScrubfuCli().Run(args);

            var fileInfo = new FileInfo(logFilePath);
            Assert.True(fileInfo.Exists);
            Assert.True(fileInfo.Length > 0);

            string logContents = File.ReadAllText(logFilePath);

            // Confirm log file contains a sample like '2019-07-09 10:01:01.213 +02:00 [INF]'
            Match matchShouldHave = Regex.Match(logContents, @"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} \[Info]", RegexOptions.Singleline);
            Assert.True(matchShouldHave.Success, string.Format("should find [Info] in log file."));

            // Confirm log file DOES NOT contains a sample like '2019-07-09 10:01:01.213 +02:00 [DBG]'
            Match matchShouldNotHave = Regex.Match(logContents, @"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [\+|\-]\d{2}:\d{2} \[Debug]", RegexOptions.Singleline);
            Assert.False(matchShouldNotHave.Success, string.Format("should NOT find [Debug] in log file."));

            File.Delete(outputFilePath);
            File.Delete(inputFilePath);
            File.Delete(logFilePath);
        }
    }
}
