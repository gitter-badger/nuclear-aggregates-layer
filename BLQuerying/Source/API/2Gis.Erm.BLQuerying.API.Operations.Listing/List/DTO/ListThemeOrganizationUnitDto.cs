using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListThemeOrganizationUnitDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public long ThemeId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}