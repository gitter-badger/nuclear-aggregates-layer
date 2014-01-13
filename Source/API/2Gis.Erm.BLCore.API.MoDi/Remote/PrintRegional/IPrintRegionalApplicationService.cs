using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.PrintRegional;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.MoDi.Remote.PrintRegional
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.MoneyDistibution.PrintRegional.ServiceContract)]
    public interface IPrintRegionalApplicationService
    {
        [OperationContract]
        PrintRegionalOrdersResponse PrintRegionalOrder(long orderId);

        [OperationContract]
        PrintRegionalOrdersResponse PrintRegionalOrders(long organizationId, DateTime startDate, DateTime endDate);
    }
}