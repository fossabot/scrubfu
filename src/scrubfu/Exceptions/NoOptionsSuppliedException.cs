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
    public sealed class NoOptionsSuppliedException : Exception, ISerializable
    {
        private const string defaultMessage = "Invalid scrubfu tag was specified.";

        public NoOptionsSuppliedException() : base(defaultMessage)
        {
        }

        public NoOptionsSuppliedException(string message) : base(!string.IsNullOrEmpty(message) ? message : defaultMessage)
        {
        }

        public NoOptionsSuppliedException(string message, Exception innerException) : base(!string.IsNullOrEmpty(message) ? message : defaultMessage, innerException)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private NoOptionsSuppliedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}