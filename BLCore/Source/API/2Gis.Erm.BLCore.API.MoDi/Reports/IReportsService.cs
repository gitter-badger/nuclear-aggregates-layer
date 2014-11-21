using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.MoDi.Reports
{
    public interface IReportsService : IOperation<ReportsServiceIdentity>
    {
        PlatformReportResponse PlatformReport(PlatformReportRequest request);
        MoneyDistributionReportResponse MoneyDistributionReport(MoneyDistributionReportRequest request);
        FileDescription ProceedsReport(DateTime startDate, DateTime endDate); 
    }
}