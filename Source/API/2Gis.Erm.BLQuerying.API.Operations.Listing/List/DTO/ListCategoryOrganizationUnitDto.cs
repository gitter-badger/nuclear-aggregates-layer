using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListCategoryOrganizationUnitDto : IListItemEntityDto<CategoryOrganizationUnit>
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
    }
}