using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListTerritoryDto : IListItemEntityDto<Territory>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public bool IsActive { get; set; }
    }
}