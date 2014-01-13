using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old
{
    public sealed class EditOrderRequest : EditRequest<Order>
    {
        public bool DiscountInPercents { get; set; }
        public long? ReservedNumberDigit { get; set; }
        public OrderState? OriginalOrderState { get; set; }
    }
}
