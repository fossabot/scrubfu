/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿
namespace Scrubfu
{
    static class Program
    {
        private static readonly ScrubfuCli cli = new ScrubfuCli();

        private static void Main(string[] args)
        {
#if DEBUG
            //Thread.Sleep(15000); // Used to give time to  attach a debugger.
#endif 

            cli.Run(args);
        }
    }
}