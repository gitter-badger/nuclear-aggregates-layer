using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListPricePositionDto : IListItemEntityDto<PricePosition>
    {
        public long Id { get; set; }
        public long PriceId { get; set; }
        public string PositionName { get; set; }
        public decimal Cost { get; set; }
        public long PlatformId { get; set; }
        public string PriceName { get; set; }
        public long PositionId { get; set; }
    }
}