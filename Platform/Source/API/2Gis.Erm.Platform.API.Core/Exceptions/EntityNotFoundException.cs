using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : BusinessLogicException
    {
        public EntityNotFoundException(Type entityType, long entityId) :
            base(GenerateMessage(entityType, entityId))
        {
        }

        public EntityNotFoundException(Type entityType) :
            base(GenerateMessage(entityType))
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

            return string.Format("Could not find {0} with id  = {1}", entityType.Name, entityId);
        }

        private static string GenerateMessage(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            return string.Format("Could not find {0}", entityType.Name);
        }
    }
}
