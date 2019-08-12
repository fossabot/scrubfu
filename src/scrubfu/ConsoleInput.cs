/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Scrubfu
{
    public sealed class ConsoleInput : IDisposable
    {
        private readonly StringReader stringReader;
        private readonly TextReader originalInput;

        public ConsoleInput()
        {
            stringReader = new StringReader(string.Empty);
            originalInput = Console.In;
            Console.SetIn(stringReader);
        }

        public string GetOuput()
        {
            return stringReader.ToString();
        }

        public void Dispose()
        {
            Console.SetIn(originalInput);
            stringReader.Dispose();
        }
    }
}
