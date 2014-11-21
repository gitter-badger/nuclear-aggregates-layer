using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Qualify
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Qualify201303)]
    public class QualifyOperationErrorDescription : IBasicOperationErrorDescription
    {
        public QualifyOperationErrorDescription(EntityName entityName, string message, long ownerCode, long? relatedEntityId)
        {
            EntityName = entityName;
            Message = message;
            OwnerCode = ownerCode;
            RelatedEntityId = relatedEntityId;
        }

        [DataMember]
        public EntityName EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public long OwnerCode { get; private set; }
        [DataMember]
        public long? RelatedEntityId { get; private set; }
    }
}