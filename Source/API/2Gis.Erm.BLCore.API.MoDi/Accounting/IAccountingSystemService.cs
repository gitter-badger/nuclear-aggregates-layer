using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.MoDi.Accounting
{
    public interface IAccountingSystemService : IOperation<ExportAccountDetailsTo1CIdentity>
    {
        ExportAccountDetailsTo1CResponse ExportAccountDetailsTo1C(long organizationId, DateTime startDate, DateTime endDate);
        ExportAccountDetailsTo1CResponse ExportAccountDetailsToServiceBus(long organizationUnitId, DateTime startDate, DateTime endDate);
    }
}