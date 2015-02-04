using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete
{
    public class CzechEvaluateBillDateService : IEvaluateBillDateService
    {
        public DateTime GetBillDate(DateTime orderCreatedOn, DateTime billPaymentDate, int billIndex)
        {
            var now = DateTime.UtcNow.Date;
            switch (billIndex)
            {
                case 0:
                    return now < billPaymentDate ? now : orderCreatedOn;
                default:
                    return billPaymentDate.GetFirstDateOfMonth();
            }
        }
    }
}
