using System;
using System.Runtime.Serialization;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class EntityIsNotUniqueException : BusinessLogicException
    {
        public Type EntityType { get; set; }

        public EntityIsNotUniqueException(Type entityType) :
            base(GenerateMessage(entityType))
        {
            EntityType = entityType;
        }

        public EntityIsNotUniqueException(Type entityType, string message)
            : base(message)
        {
            EntityType = entityType;
        }

        protected EntityIsNotUniqueException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        private static string GenerateMessage(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            return string.Format("Сущность {0} не уникальна", entityType.AsEntityName());
        }
    }
}