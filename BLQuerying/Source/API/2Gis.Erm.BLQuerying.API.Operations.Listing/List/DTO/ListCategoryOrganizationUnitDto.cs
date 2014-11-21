using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListCategoryOrganizationUnitDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }

        public bool IsDeleted { get; set; }
        public bool CategoryIsDeleted { get; set; }
    }
}