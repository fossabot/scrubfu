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
    public sealed class MalformedSqlParseErrorException : SqlParseErrorException, ISerializable
    {
        private const string what = "SQL Entity Name";

        public MalformedSqlParseErrorException() : base(what)
        {
        }

        public MalformedSqlParseErrorException(string reason) : base(what, reason)
        {
        }

        public MalformedSqlParseErrorException(string reason, Exception innerException) : base(what, reason, innerException)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private MalformedSqlParseErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}