using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Common.Exceptions
{
    [Serializable]
    public class InactiveEntityModificationException : BusinessLogicException
    {
        public InactiveEntityModificationException() :
            base(GenerateMessage())
        {
        }

        public InactiveEntityModificationException(string message) :
            base(message)
        {
        }

        protected InactiveEntityModificationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        // TODO {all, 15.10.2014}: Выглядит подозрительным перекладывание ответственности за формирование Message на класс исключения
        private static string GenerateMessage()
        {
            return BLResources.EntityIsInactive;
        }
    }
}