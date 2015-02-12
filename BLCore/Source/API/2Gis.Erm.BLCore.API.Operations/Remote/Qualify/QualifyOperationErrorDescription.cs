using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Qualify
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Qualify201303)]
    public class QualifyOperationErrorDescription : IBasicOperationErrorDescription
    {
        public QualifyOperationErrorDescription(IEntityType entityName, string message, long ownerCode, long? relatedEntityId)
        {
            EntityName = entityName.Description;
            Message = message;
            OwnerCode = ownerCode;
            RelatedEntityId = relatedEntityId;
        }

        [DataMember]
        public string EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public long OwnerCode { get; private set; }
        [DataMember]
        public long? RelatedEntityId { get; private set; }
    }
}