using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListCategoryGroupDto : IListItemEntityDto<CategoryGroup>
    {
        public long Id { get; set; }
        public string CategoryGroupName { get; set; }
        public decimal GroupRate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}