using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.MoDi.PrintRegional
{
    public interface IPrintRegionalOrderService : IOperation<PrintRegionalOrderIdentity>
    {
        PrintRegionalOrdersResponse PrintRegionalOrder(long orderId);
        PrintRegionalOrdersResponse PrintRegionalOrder(long organizationId, DateTime startDate, DateTime endDate);
    }
}