/**
 * *************************************************
 * Copyright (c) 2019, Grindrod Bank Limited
 * License MIT: https://opensource.org/licenses/MIT
 * **************************************************
 */
﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Scrubfu.Exceptions
{
    [Serializable]
    public class SqlParseErrorException : Exception, ISerializable
    {
        private const string DefaultMessage = "Could not parse SQL.";
        private const string WhatWithNoReasonMessage = "Could not parse {0}.";
        private const string WhatWithReasonMessage = "Could not parse {0}: {1}";

        public SqlParseErrorException() : base(DefaultMessage)
        {
        }

        public SqlParseErrorException(string what) : base(!string.IsNullOrEmpty(what)
            ? string.Format(WhatWithNoReasonMessage, what)
            : DefaultMessage)
        {
        }

        public SqlParseErrorException(string what, string reason) : base(
            !string.IsNullOrEmpty(what) && !string.IsNullOrEmpty(reason)
                ? string.Format(WhatWithReasonMessage, what, reason)
                : DefaultMessage)
        {
        }

        public SqlParseErrorException(string what, string reason, Exception innerException) : base(
            !string.IsNullOrEmpty(what) && !string.IsNullOrEmpty(reason)
                ? string.Format(WhatWithReasonMessage, what, reason)
                : DefaultMessage, innerException)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected SqlParseErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}