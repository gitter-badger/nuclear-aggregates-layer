using System.Collections.Generic;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.OrderProcessing
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
    public interface IRequestStateApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(OrderProcessingErrorDescription), Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
        IOrderRequestStateDescription[] GetState(IEnumerable<long> requestIds);
    }
}
