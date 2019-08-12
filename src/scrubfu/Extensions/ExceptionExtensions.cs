/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.Text;

namespace Scrubfu.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ExtractEntireErrorMessage(this Exception ex, bool withStackTrace = false)
        {
            Exception localEx = ex;

            StringBuilder returnMessageSb = new StringBuilder();
            while (localEx != null)
            {
                returnMessageSb.Append(localEx.Message.ToString()).Append(" ");
                localEx = localEx.InnerException;
            }

            if (withStackTrace && ex != null)
                returnMessageSb.AppendFormat("Stack Trace: {0}.", ex.StackTrace);

            return returnMessageSb.ToString();
        }
    }
}