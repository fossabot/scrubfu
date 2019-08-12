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
    public sealed class SqlEntityNameParseErrorException : SqlParseErrorException, ISerializable
    {
        private const string what = "SQL Entity Name";

        public SqlEntityNameParseErrorException() : base(what)
        {
        }

        public SqlEntityNameParseErrorException(string reason) : base(what, reason)
        {
        }

        public SqlEntityNameParseErrorException(string reason, Exception innerException) : base(what, reason,
            innerException)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private SqlEntityNameParseErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}