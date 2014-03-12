using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListPlatformDto : IListItemEntityDto<Platform.Model.Entities.Erm.Platform>
    {
        public long Id { get; set; }
        public string Name{ get; set; }
    }
}