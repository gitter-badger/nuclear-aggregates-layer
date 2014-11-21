using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Append
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Append201303)]
    public class AppendOperationErrorDescription : IBasicOperationErrorDescription
    {
        public AppendOperationErrorDescription(EntityName entityName, long entityId, EntityName appendedEntityName, long appendedEntityId, string message)
        {
            EntityName = entityName;
            EntityId = entityId;
            AppendedEntityName = appendedEntityName;
            AppendedEntityId = appendedEntityId;
            Message = message;
        }

        [DataMember]
        public EntityName EntityName { get; private set; }
        [DataMember]
        public long EntityId { get; set; }
        [DataMember]
        public EntityName AppendedEntityName { get; set; }
        [DataMember]
        public long AppendedEntityId { get; set; }
        [DataMember]
        public string Message { get; private set; }
    }
}