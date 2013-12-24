using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.Core.Exceptions
{
    // TODO {d.ivanov, 29.11.2013}: можно положить в 2Gis.Erm.Platform.API.Core\Exceptions\EntityNotFoundException.cs, рядом с BusinessLogicException
    [Serializable]
    public class EntityNotFoundException : BusinessLogicException
    {
        public EntityNotFoundException(Type entityType, long entityId) :
            base(GenerateMessage(entityType, entityId))
        {
        }

        protected EntityNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        private static string GenerateMessage(Type entityType, long entityId)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            return string.Format("Сущность {0}.Id = {1} не найдена", entityType.Name, entityId);
        }
    }
}
