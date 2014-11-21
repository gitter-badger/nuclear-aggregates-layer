using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeTerritory
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace =  ServiceNamespaces.BasicOperations.ChangeTerritory201303)]
    public interface IChangeTerritoryApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ChangeTerritoryOperationErrorDescription), Namespace =  ServiceNamespaces.BasicOperations.ChangeTerritory201303)]
        void Execute(EntityName entityName, long entityId, long territoryId);
    }
}