/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.IO;

namespace Scrubfu.Tests
{
    public class ConsoleErrorOutput : IDisposable
    {
        private StringWriter stringWriter;
        private TextWriter originalOutput;

        public ConsoleErrorOutput()
        {
            stringWriter = new StringWriter();
            originalOutput = Console.Error;
            Console.SetError(stringWriter);
        }

        public string GetOuput()
        {
            return stringWriter.ToString();
        }

        public void Dispose()
        {
            Console.SetError(originalOutput);
            stringWriter.Dispose();
        }
    }
}
