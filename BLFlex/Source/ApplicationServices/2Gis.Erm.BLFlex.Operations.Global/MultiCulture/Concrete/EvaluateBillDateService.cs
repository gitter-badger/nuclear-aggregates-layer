using System;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete
{
    public class EvaluateBillDateService : IEvaluateBillDateService
    {
        public DateTime GetBillDate(DateTime orderCreatedOn, DateTime billPaymentDate, int billIndex)
        {
            return orderCreatedOn;
        }
    }
}
