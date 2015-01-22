using System.Collections.Generic;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.Platform.API.Core;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.CheckForDebts
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.CheckForDebts201303)]
    public interface ICheckForDebtsApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(CheckForDebtsOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.CheckForDebts201303)]
        CheckForDebtsResult Execute(IEntityType entityName, IEnumerable<long> entityIds);
    }
}