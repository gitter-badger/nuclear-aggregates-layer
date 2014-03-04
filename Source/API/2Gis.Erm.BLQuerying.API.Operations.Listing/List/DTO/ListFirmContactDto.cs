using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListFirmContactDto : IListItemEntityDto<FirmContact>
    {
        public long Id { get; set; }
        public string ContactType { get; set; }
        public string Contact { get; set; }
        public long? CardId { get; set; }
    }
}