using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeTerritory
{
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.ChangeTerritory201303)]
    public class ChangeTerritoryOperationErrorDescription : IBasicOperationErrorDescription
    {
        public ChangeTerritoryOperationErrorDescription(IEntityType entityName, string message, long entityId, long territoryId)
        {
            EntityName = entityName.Description;
            Message = message;
            EntityId = entityId;
            TerritoryId = territoryId;
        }

        [DataMember]
        public string EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        [DataMember]
        public long EntityId { get; private set; }
        [DataMember]
        public long TerritoryId { get; private set; }
    }
}