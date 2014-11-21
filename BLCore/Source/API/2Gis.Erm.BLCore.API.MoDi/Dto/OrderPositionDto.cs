using DoubleGis.Erm.BLCore.API.MoDi.Enums;

namespace DoubleGis.Erm.BLCore.API.MoDi.Dto
{
    public sealed class OrderPositionDto
    {
        public long PositionId { get; set; }
        public long PriceId { get; set; }

        public int Amount { get; set; }
        public bool CalculateDiscountViaPercent { get; set; }
        public decimal DiscountSum { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal PricePositionCost { get; set; }

        public PlatformsExtended ExtendedPlatform { get; set; }

        public decimal CategoryRate { get; set; }
    }
}