using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.Reports;
using DoubleGis.Erm.BLCore.API.MoDi.Reports;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.MoDi
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ReportsApplicationService : IReportsApplicationService
    {
        private readonly IReportsService _reportsService;

        public ReportsApplicationService(IUserContext userContext, IReportsService reportsService, ICommonLog logger)
        {
            _reportsService = reportsService;

            logger.InfoFormat("Печатается отчёт от имени пользователя {0} ({1})", userContext.Identity.DisplayName, userContext.Identity.Account);
            EnumResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
        }

        public PlatformReportResponse PlatformReport(PlatformReportRequest request)
        {
            return _reportsService.PlatformReport(request);
        }

        public MoneyDistributionReportResponse MoneyDistributionReport(MoneyDistributionReportRequest request)
        {
            return _reportsService.MoneyDistributionReport(request);
        }

        public FileDescription ProceedsReport(DateTime startDate, DateTime endDate)
        {
            return _reportsService.ProceedsReport(startDate, endDate);
        }
    }
}
