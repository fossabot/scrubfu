/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Scrubfu.Exceptions;
using Scrubfu.Models;
using Scrubfu.Services;
using NLog;
using NLog.Config;
using NLog.Targets;

using System.Text;

namespace Scrubfu
{
    public class ScrubfuCli
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        
        private bool loggingToConsole;

        public void Run(string[] args)
        {
            ConsoleInput redirectedInput = null;
            CliInput cliInput = null;
            SetupInitialIO(ref redirectedInput);

            try
            {
                if (!CheckEmptyArgs(args))
                    return;

                cliInput = new ArgumentParser(args).ParseArgs();

                if (!HandleOptions(cliInput))
                    return;

                ScrubFile(cliInput);
            }
            catch (Exception ex)
            {
                if (!loggingToConsole)
                    logger.Error(ex, ex.Message);

                if (ex is ArgumentException ||
                    ex is ArgumentNullException ||
                    ex is ArgumentOutOfRangeException)
                {
                    Console.Error.WriteLine(InvalidInputMessage(string.Format("Error: {0}\r\n", ex.Message.ToString())));
                    return;
                }

                // Unhandled exceptions
                Console.Error.WriteLine("Error: An unhandled error has occurred. Please check the log file for more details.");
            }

            // Logging complete
            NLog.LogManager.Shutdown();

            if (redirectedInput != null)
                redirectedInput.Dispose();
        }

        private void SetupInitialIO(ref ConsoleInput redirectedInput)
        {
            SetupConsoleLogger();
            HandleValidRedirectedInput(ref redirectedInput);
        }

        private void SetupConsoleLogger()
        {
            if (!Console.IsOutputRedirected)
            {
                var config = new LoggingConfiguration();
                var logConsole = new ConsoleTarget("logconsole")
                {
                    Layout = @"${message} ${exception}"  
                };

                config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
                NLog.LogManager.Configuration = config;

                loggingToConsole = true;
            }

        }

        private void HandleValidRedirectedInput(ref ConsoleInput redirectedInput)
        {
#if DEBUG
            const int STD_WAIT_TIME = 500; 

            // Test for invalid in stream and replace with empty Text reader 
            if (Console.IsInputRedirected)
            {
                var inputReceived = new AutoResetEvent(false);
                var inputThread = new Thread(() =>
                {
                    try
                    {
                        Console.In.Peek();
                        inputReceived.Set();
                    }
                    catch (ThreadAbortException)
                    {
                        Thread.ResetAbort();
                    }
                })
                {
                    IsBackground = true
                };

                inputThread.Start();

                // Timeout expired?
                if (!inputReceived.WaitOne(STD_WAIT_TIME))
                {
                    // Gracefully stop the thread causing internal error.
                    redirectedInput = new ConsoleInput();
                }
            }
#endif 
        }

        private bool CheckEmptyArgs(string[] args)
        {
            if ((Console.IsInputRedirected && (Console.In.Peek() == -1)) && (args.Length == 0 || string.IsNullOrWhiteSpace(string.Join(' ', args)))
                || (!Console.IsInputRedirected && (args.Length == 0 || string.IsNullOrWhiteSpace(string.Join(' ', args)))))
            {
                Console.WriteLine(GetUsage());
                return false;
            }

            return true;
        }

        private string InvalidInputMessage(string errorMessage)
        {
            return string.Format("{0}\r\nRun --help to get usage information.", errorMessage);
        }

        private bool HandleOptions(CliInput cliInput)
        {
            if (string.IsNullOrWhiteSpace(cliInput.OutFile))
                NLog.LogManager.Shutdown();

            if (cliInput.SpecifyingLogFile)
                CreateFileLogger(cliInput);

            if (cliInput.AskingHelp)
            {
                logger.Debug($"Processing Help...");
                Console.WriteLine(GetUsage());
                return false;
            }

            if (cliInput.AskingVersion)
            {
                logger.Debug($"Processing Version...");
                Console.WriteLine(GetVersionInformation());
                return false;
            }

            return true;
        }

        private void CreateFileLogger(CliInput cliInput)
        {
            var config = new LoggingConfiguration();
            var logFile = new FileTarget("logfile")
            {
                FileName = cliInput.LogfileLocation,
                Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} [${level}] ${message} ${exception}"  
            };

            // Still log errors and fatals to console
            var logConsole = new ConsoleTarget("logconsole")
            {
                Layout = @"${message} ${exception}"  
            };

            config.AddRule(LogLevel.Error, LogLevel.Fatal, logConsole);

            LogLevel fileLogLevel = LogLevel.Info;
            switch (cliInput.Loglevel.ToLower())
            {
                case "error":
                    fileLogLevel = LogLevel.Error;
                    break;
                case "debug":
                    fileLogLevel = LogLevel.Debug;
                    break;
            }
            config.AddRule(fileLogLevel, LogLevel.Fatal, logFile);

            NLog.LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();

            loggingToConsole = false;
        }

        private void ScrubFile(CliInput cliInput)
        {
            bool inIsFile = false;
            bool outIsFile = false;
            TextReader inReader = null;
            TextWriter outWriter = null;

            try
            {
                logger.Info("Executing scrub...");

                PrepareStreams(ref inReader, ref outWriter, ref inIsFile, ref outIsFile, cliInput);

                var scrubber = new Scrubber(inReader, outWriter);
                var scrubTimer = new Stopwatch();
                scrubTimer.Start();
                scrubber.Scrub();
                scrubTimer.Stop();

                GenerateCompletedOutput(scrubber.Errors.Count, scrubber.Warnings.Count, scrubTimer);
            }
            catch (SqlParseErrorException ex)
            {
                LogError(ex);
                Console.Error.WriteLine(string.Format("Scrub validation error: {0}", ex.Message));
            }
            catch (Exception ex)
            {
                LogError(ex);
                Console.Error.WriteLine("Scrub failure: An unhandled error occurred during scrubbing. Please check the log file for more details.");
            }
            
            CleanupStreams(ref inReader, ref outWriter, ref inIsFile, ref outIsFile);
        }

        private void GenerateCompletedOutput(int errorCount, int warningCount, Stopwatch scrubTimer)
        {
            StringBuilder outputSb = new StringBuilder("Scrub completed");

            if (errorCount > 0)
                outputSb.Append(string.Format(" with {0} errors", errorCount));
            
            if (warningCount > 0)
            {
                if (errorCount > 0)
                    outputSb.Append(" and");

                outputSb.Append(string.Format(" with {0} warnings", warningCount));
            }

            outputSb.Append(string.Format(" in {0}.", GetFormattedElapsedTime(scrubTimer)));

            if (errorCount > 0 || warningCount > 0)
                outputSb.Append(" Please check the log for more information.");

            LogCompleteOutput(outputSb.ToString(), errorCount, warningCount);
        }
        
        private void LogCompleteOutput(string output, int errorCount, int warningCount)
        {
            if (errorCount > 0)
                logger.Error(output);
            else if (warningCount > 0)
                logger.Warn(output);
            else
                logger.Info(output);
        }

        private string GetFormattedElapsedTime(Stopwatch scrubTimer)
        {
            if (scrubTimer.Elapsed.TotalMilliseconds < 1000)     // Less than 1 second
                return string.Format("{0} milliseconds", (int)scrubTimer.Elapsed.TotalMilliseconds);

            if (scrubTimer.Elapsed.TotalSeconds < 60)    // Less than 1 minute
                return string.Format("{0}.{1} seconds", (int)scrubTimer.Elapsed.TotalSeconds, scrubTimer.Elapsed.Milliseconds);

            if (scrubTimer.Elapsed.TotalMinutes < 60)    // Less than 1 hour
                return string.Format("{0} minutes {1} seconds", (int)scrubTimer.Elapsed.TotalMinutes, scrubTimer.Elapsed.Seconds);

            if (scrubTimer.Elapsed.TotalHours < 24)    // Less than 1 day
                return string.Format("{0} hours {1} minutes", (int)scrubTimer.Elapsed.TotalHours, scrubTimer.Elapsed.Minutes);

            // More than a day
            return string.Format("{0} days {1} hours {2} minutes", (int)scrubTimer.Elapsed.TotalDays, scrubTimer.Elapsed.Hours, scrubTimer.Elapsed.Minutes);
        }

        private void LogError(Exception ex)
        {
            if (!loggingToConsole)
                logger.Error(ex, ex.Message);
        }

        private void PrepareStreams(ref TextReader inReader, ref TextWriter outWriter, ref bool inIsFile, ref bool outIsFile, CliInput cliInput)
        {
            // Prepare input object
            if (!string.IsNullOrWhiteSpace(cliInput.InFile))
            {
                inReader = File.OpenText(cliInput.InFile);
                inIsFile = true;
            }
            else if (Console.IsInputRedirected)
                inReader = Console.In;
            else
                throw new ArgumentException("Input not valid.");

            // Prepare input object
            if (!string.IsNullOrWhiteSpace(cliInput.OutFile))
            {
                outWriter = File.CreateText(cliInput.OutFile);
                outIsFile = true;
            }
            else
                outWriter = Console.Out;
        }

        private void CleanupStreams(ref TextReader inReader, ref TextWriter outWriter,  ref bool inIsFile, ref bool outIsFile)
        {
            if (inIsFile && inReader != null)
            {
                inReader.Close();
                inReader.Dispose();
            }

            if (outIsFile && outWriter != null)
            {
                outWriter.Close();
                outWriter.Dispose();
            }
        }

        private string GetVersionInformation()
        {
            var appName = "Scrubfu";
            var appVersionNumber = GetAssemblyVersion();

            return ($"{appName} v{appVersionNumber}");
        }

        private string GetUsage()
        {
            return (string.Format("Usage: scrubfu [OPTIONS] [INFILE] [OUTFILE]\r\n" +
                                "\r\n" +
                                "  scrubfu {0}, Copyright(c) 2019, Grindrod Bank Limited\r\n" +
                                "\r\n" +
                                "     Distributed under the MIT license.\r\n" +
                                "\r\n" +
                                "  Scrubfu is a .NET CLI tool that makes creating usable development data from\r\n" +
                                "  production data a predictable, audit-able and repeatable process. The\r\n" +
                                "  script works by scrubbing and /or obfuscating table column data based on\r\n" +
                                "  script-tags in SQL comments stored in the PostgreSQL database.\r\n" +
                                "\r\n" +
                                "  [INFILE] is the input file obtained by a pg_dump with --inserts.\r\n" +
                                "\r\n" +
                                "  [OUTFILE] is the scrubfu'ed file, ready to be imported with pg_import.\r\n" +
                                "\r\n" +
                                "  For further details see: https://github.com/GrindrodBank/scrubfu\r\n" +
                                "" +
                                "Options:\r\n" +
                                "  -h, --help                      Show this message and exit.\r\n" +
                                "  -v, --version                   Show the version and exit.\r\n" +
                                "  --log TEXT                      Optional LOGFILE, defaults to standard out.\r\n" +
                                "  --log_level [error|info|debug]  Used with [--log=LOGFILE].\r\n" //+
                                //"  -k, --ref_fk                        Flag: also scrubfu related foreign key data.\r\n"
                                , GetAssemblyVersion()));
        }

        private string GetAssemblyVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var appVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return string.Format("{0}.{1}.{2}", appVersionInfo.ProductMajorPart, appVersionInfo.ProductMinorPart, appVersionInfo.ProductBuildPart);
        }
    }
}
