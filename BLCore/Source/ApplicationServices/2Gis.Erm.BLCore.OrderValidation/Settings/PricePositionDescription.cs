using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;

namespace DoubleGis.Erm.BLCore.OrderValidation.Settings
{
    public sealed class PricePositionDescription
    {
        public long PositionId { get; set; }
        public PricePositionDto.RelatedItemDto[] MasterPositions { get; set; }
        public PricePositionDto.RelatedItemDto[] DeniedPositions { get; set; }
    }
}
