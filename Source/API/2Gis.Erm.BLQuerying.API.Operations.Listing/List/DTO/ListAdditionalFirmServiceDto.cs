using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdditionalFirmServiceDto : IListItemEntityDto<AdditionalFirmService>
    {
        public long Id { get; set; }
        public string ServiceCode { get; set; }
        public string Description { get; set; }
        public bool IsManaged { get; set; }
    }
}