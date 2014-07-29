using DoubleGis.Erm.BLCore.API.MoDi.Enums;

namespace DoubleGis.Erm.BLCore.API.MoDi.Dto
{
    public sealed class PlatformCost
    {
        // позиция номенклатуры
        public long PositionId { get; set; }

        // направление продаж
        public ESalesPointType From { get; set; }
        public ESalesPointType To { get; set; }
        public PlatformsExtended Platform { get; set; }
        public ESalesPointType NewTo { get; set; }

        // региональные деньги
        public decimal Cost { get; set; }
        public decimal DiscountCost { get; set; }

        // клиентские деньги
        public decimal PayablePlan { get; set; }
        public decimal PayablePrice { get; set; }
    }
}