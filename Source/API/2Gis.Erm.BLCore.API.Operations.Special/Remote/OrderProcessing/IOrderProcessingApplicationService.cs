using System;
using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.OrderProcessing
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
    public interface IOrderProcessingRequestsApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(OrderProcessingErrorDescription), Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
        long ProlongateOrder(long orderId, short releaseCountPlan);

        [OperationContract]
        [FaultContract(typeof(OrderProcessingErrorDescription), Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
        long ProlongateOrderWithComment(long orderId, short releaseCountPlan, string description);

        [OperationContract]
        [FaultContract(typeof(OrderProcessingErrorDescription), Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
        long CreateOrder(long sourceProjectCode,
                         DateTime beginDistributionDate,
                         short releaseCountPlan,
                         long firmId,
                         long legalPersonProfileId,
                         string description);
    }
}
