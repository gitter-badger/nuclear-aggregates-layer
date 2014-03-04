using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListThemeOrganizationUnitDto : IListItemEntityDto<ThemeOrganizationUnit>
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public bool IsDeleted { get; set; }
        public long ThemeId { get; set; }
    }
}