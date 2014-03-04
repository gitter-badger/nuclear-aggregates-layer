using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListOrderPositionAdvertisementDto : IListItemEntityDto<OrderPositionAdvertisement>
    {
        public long Id { get; set; }
        public long OrderPositionId { get; set; }
        public long PositionId { get; set; }
        public long? AdvertisementId { get; set; }
        public long? FirmAddressId { get; set; }
        public long? CategoryId { get; set; }
        public long? ThemeId { get; set; }
    }
}