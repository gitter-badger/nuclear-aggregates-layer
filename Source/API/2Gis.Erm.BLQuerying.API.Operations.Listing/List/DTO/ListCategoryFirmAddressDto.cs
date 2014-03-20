using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListCategoryFirmAddressDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
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
        public long FirmId { get; set; }
    }
}
