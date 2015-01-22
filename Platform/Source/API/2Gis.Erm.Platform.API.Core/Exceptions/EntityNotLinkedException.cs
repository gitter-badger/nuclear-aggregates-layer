using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.Core.Exceptions
{
    [Serializable]
    public class EntityNotLinkedException : BusinessLogicException
    {
        public EntityNotLinkedException(Type mainEntityType, long mainEntityId, Type linkedEntity) :
            base(GenerateMessage(mainEntityType, mainEntityId, linkedEntity))
        {
        }

        public EntityNotLinkedException(string message) :
            base(message)
        {
        }

        protected EntityNotLinkedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        private static string GenerateMessage(Type mainEntityType, long mainEntityId, Type linkedEntity)
        {
            if (mainEntityType == null)
            {
                throw new ArgumentNullException("mainEntityType");
            }

            if (linkedEntity == null)
            {
                throw new ArgumentNullException("linkedEntity");
            }

            return string.Format("Entity {0} with id {1} does not contain link to {2}", mainEntityType.Name, mainEntityId, linkedEntity.Name);
        }
    }
}
