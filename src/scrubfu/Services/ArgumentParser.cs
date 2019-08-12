/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.IO;
using System.Linq;
using Scrubfu.Models;

namespace Scrubfu.Services
{
    public class ArgumentParser
    {
        private readonly string[] args;

        public ArgumentParser(string[] args)
        {
            this.args = args;
        }

        public CliInput ParseArgs()
        {
            var inputObject = new CliInput();

            CliOption newOption = null;
            foreach (var arg in args)
                ParseArg(arg, ref newOption, ref inputObject);

            if (newOption != null)
                inputObject.options.Add(newOption);

            ValidateInputObject(inputObject);

            return inputObject;
        }

        private void ParseArg(string arg, ref CliOption newOption, ref CliInput inputObject)
        {
            if (arg.Trim() == "-")
                return; // - indicates std out
                
            if (arg.Trim().StartsWith("-", StringComparison.Ordinal) || arg.Trim().StartsWith("--", StringComparison.Ordinal))
                CreateNewOption(arg, ref newOption, ref inputObject);
            else   // Possibly dealing with previous option values
                AddCompletedOption(arg, ref newOption, ref inputObject);
        }

        private void CreateNewOption(string arg, ref CliOption newOption, ref CliInput inputObject)
        {
            if (!Constants.GetValidCommands().Contains(arg.Trim()))
                throw new ArgumentException(string.Format("Argument {0} is not valid.", arg.Trim()));

            if (newOption != null) // Previous command has been constructed
                inputObject.options.Add(newOption);

            newOption = new CliOption
            {
                Name = arg.Trim()
            };
        }

        private void AddCompletedOption(string arg, ref CliOption newOption, ref CliInput inputObject)
        {
            if (newOption != null && newOption.ShouldHaveValue())
            {
                newOption.Value = arg;
                inputObject.options.Add(newOption);
                newOption = null;
            }
            else
            {
                // Not a option value, so check if this is a file
                newOption = null;

                if (String.IsNullOrWhiteSpace(inputObject.InFile) && (!Console.IsInputRedirected || (Console.IsInputRedirected && Console.In.Peek() == -1)))
                    inputObject.InFile = arg.Trim();
                else
                    inputObject.OutFile = arg.Trim();
            }
        }

        private void ValidateInputObject(CliInput inputObject)
        {
            ValidateOptions(inputObject);
            ValidationPaths(inputObject);
        }

        private void ValidateOptions(CliInput inputObject)
        {
            foreach (CliOption option in inputObject.options)
            {
                switch (option.Name)
                {
                    case Constants.LogCliCommandOption:
                    case Constants.LogCliCommandOptionShortHand:
                        if (!IsValidPath(option.Value))
                            throw new ArgumentException("Log file not a valid file location.");
                        break;

                    case Constants.LogLevelCliCommandOption:
                        if (!IsValidLogLevel(option.Value))
                            throw new ArgumentException(string.Format("Log level specified is not valid. Valid options are {0}",
                                Constants.LogLevelCliCommandOptionValues));
                        break;
                }
            }
        }

        private void ValidationPaths(CliInput inputObject)
        {
            if (!inputObject.AskingHelp && !inputObject.AskingVersion)
            {
                if (!IsValidPath(inputObject.InFile) && !Console.IsInputRedirected)
                    throw new ArgumentException("Input not valid.");

                if (!String.IsNullOrEmpty(inputObject.OutFile) && !IsValidPath(inputObject.OutFile) && !Console.IsOutputRedirected)
                    throw new ArgumentException("Output not valid.");

                if (!string.IsNullOrWhiteSpace(inputObject.InFile) && !File.Exists(inputObject.InFile))
                    throw new ArgumentException("Input file does not exist.");
            }
        }

        private bool IsValidPath(string path)
        {
            try
            {
                return (Path.GetFullPath(path) != string.Empty);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsValidLogLevel(string logLevel)
        {
            return (Constants.LogLevelCliCommandOptionValues.Split('|').ToList().Contains(logLevel));
        }
    }
}
