using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.Accounting;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.AccountingSystem;

namespace DoubleGis.Erm.BLCore.WCF.MoDi
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class AccountingSystemApplicationService : IAccountingSystemApplicationService
    {
        private readonly IAccountingSystemService _accountingSystemService;
        public AccountingSystemApplicationService(IAccountingSystemService accountingSystemService)
        {
            _accountingSystemService = accountingSystemService;
        }

        public ExportAccountDetailsTo1CResponse ExportAccountDetailsTo1C(long organizationId, DateTime startDate, DateTime endDate)
        {
            return _accountingSystemService.ExportAccountDetailsTo1C(organizationId, startDate, endDate);
        }

        public ExportAccountDetailsTo1CResponse ExportAccountDetailsToServiceBus(long organizationId, DateTime startDate, DateTime endDate)
        {
            return _accountingSystemService.ExportAccountDetailsToServiceBus(organizationId, startDate, endDate);
        }
    }
}
