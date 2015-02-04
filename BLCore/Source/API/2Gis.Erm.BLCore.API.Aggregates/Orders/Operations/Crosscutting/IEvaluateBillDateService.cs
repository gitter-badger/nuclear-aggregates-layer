using System;

using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public interface IEvaluateBillDateService : IInvariantSafeCrosscuttingService
    {
        DateTime GetBillDate(DateTime orderCreatedOn, DateTime billPaymentDate, int billIndex);
    }
}
