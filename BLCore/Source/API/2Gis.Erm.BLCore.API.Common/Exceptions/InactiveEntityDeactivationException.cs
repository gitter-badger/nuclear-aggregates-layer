using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Common.Exceptions
{
    [Serializable]
    public class InactiveEntityDeactivationException : BusinessLogicException
    {
        public InactiveEntityDeactivationException(Type entityType, long entityId) :
            base(GenerateMessage(entityType, entityId.ToString()))
        {
        }

        public InactiveEntityDeactivationException(Type entityType, string name) :
            base(GenerateMessage(entityType, name))
        {
        }

        protected InactiveEntityDeactivationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        // TODO {all, 15.10.2014}: Выглядит подозрительным перекладывание ответственности за формирование Message на класс исключения
        private static string GenerateMessage(Type entityType, string name)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            return string.Format(BLResources.EntityIsAlreadyInactive, entityType.AsEntityName().ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture), name);
        }
    }
}