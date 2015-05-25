using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Append
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.Append201303)]
    public class AppendOperationErrorDescription : IBasicOperationErrorDescription
    {
        public AppendOperationErrorDescription(IEntityType entityName, long entityId, IEntityType appendedEntityName, long appendedEntityId, string message)
        {
            EntityName = entityName.Description;
            EntityId = entityId;
            AppendedEntityName = appendedEntityName.Description;
            AppendedEntityId = appendedEntityId;
            Message = message;
        }

        [DataMember]
        public string EntityName { get; private set; }
        [DataMember]
        public long EntityId { get; set; }
        [DataMember]
        public string AppendedEntityName { get; set; }
        [DataMember]
        public long AppendedEntityId { get; set; }
        [DataMember]
        public string Message { get; private set; }
    }
}