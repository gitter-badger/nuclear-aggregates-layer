using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.Accounting;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.MoDi.Remote.AccountingSystem
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.MoneyDistibution.AccountingSystem.ServiceContract)]
    public interface IAccountingSystemApplicationService
    {
        [OperationContract]
        ExportAccountDetailsTo1CResponse ExportAccountDetailsTo1C(long organizationId, DateTime startDate, DateTime endDate);

        [OperationContract]
        ExportAccountDetailsTo1CResponse ExportAccountDetailsToServiceBus(long organizationId, DateTime startDate, DateTime endDate); 
    }
}