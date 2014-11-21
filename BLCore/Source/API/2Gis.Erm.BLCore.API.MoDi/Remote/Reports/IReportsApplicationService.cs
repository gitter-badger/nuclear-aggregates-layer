using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.Reports;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.MoDi.Remote.Reports
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.MoneyDistibution.Reports.ServiceContract)]
    public interface IReportsApplicationService
    {
        [OperationContract]
        PlatformReportResponse PlatformReport(PlatformReportRequest request);
        [OperationContract]
        MoneyDistributionReportResponse MoneyDistributionReport(MoneyDistributionReportRequest request);
        [OperationContract]
        FileDescription ProceedsReport(DateTime startDate, DateTime endDate); 
    }
}