using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListPositionCategoryDto : IListItemEntityDto<PositionCategory>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int ExportCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}