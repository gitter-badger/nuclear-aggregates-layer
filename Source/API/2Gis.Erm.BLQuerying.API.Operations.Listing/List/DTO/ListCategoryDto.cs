using DoubleGis.Erm.BLCore.API.Operations.Generic.List;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListCategoryDto : IListItemEntityDto<Erm.Platform.Model.Entities.Erm.Category>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string ParentName { get; set; }
        public int Level { get; set; }
    }
}