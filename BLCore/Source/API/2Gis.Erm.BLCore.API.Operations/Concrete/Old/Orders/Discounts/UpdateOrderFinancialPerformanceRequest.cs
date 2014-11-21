using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts
{
    public sealed class UpdateOrderFinancialPerformanceRequest : Request
    {
        // from order position
        public Order Order { get; set; }

        // from order
        public bool RecalculateFromOrder { get; set; }
        public bool OrderDiscountInPercents { get; set; }
        public int ReleaseCountFact { get; set; }
    }
}