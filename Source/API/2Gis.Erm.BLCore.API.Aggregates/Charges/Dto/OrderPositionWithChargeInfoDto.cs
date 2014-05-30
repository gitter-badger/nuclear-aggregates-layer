using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto
{
    public sealed class OrderPositionWithChargeInfoDto
    {
        public Lock Lock { get; set; }

        public ChargeInfoDto ChargeInfo { get; set; }
        public OrderInfoDto OrderInfo { get; set; }
        public OrderPositionInfoDto OrderPositionInfo { get; set; }
    }
}