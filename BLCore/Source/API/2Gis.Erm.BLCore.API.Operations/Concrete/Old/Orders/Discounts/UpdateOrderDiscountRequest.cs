using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts
{
    public sealed class UpdateOrderDiscountRequest : Request
    {
        public Order Order { get; set; }
        public bool DiscountInPercents { get; set; }
    }
}
