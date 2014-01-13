using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListCategoryFirmAddressDto : IListItemEntityDto<CategoryFirmAddress>
    {
        public long CategoryId { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string ParentName { get; set; }
        public int Level { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public long FirmAddressId { get; set; }
        public int SortingPosition { get; set; }
        public string CategoryGroup { get; set; }
    }
}
