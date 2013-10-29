﻿using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class BusinessLogicException : ErmException
    {
        public BusinessLogicException() { }
        public BusinessLogicException(String message) : base(message) { }
        public BusinessLogicException(String message, Exception innerException) : base(message, innerException) { }
        protected BusinessLogicException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}