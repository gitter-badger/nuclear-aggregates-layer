using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeTerritory
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.ChangeTerritory201303)]
    public class ChangeTerritoryOperationErrorDescription : IBasicOperationErrorDescription
    {
        public ChangeTerritoryOperationErrorDescription(EntityName entityName, string message, long entityId, long territoryId)
        {
            EntityName = entityName;
            Message = message;
            EntityId = entityId;
            TerritoryId = territoryId;
        }

        [DataMember]
        public EntityName EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public long EntityId { get; private set; }
        [DataMember]
        public long TerritoryId { get; private set; }
    }
}