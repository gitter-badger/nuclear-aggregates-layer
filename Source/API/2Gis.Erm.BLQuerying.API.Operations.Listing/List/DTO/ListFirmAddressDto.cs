using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListFirmAddressDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public int SortingPosition { get; set; }
        public string FirmName { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool ClosedForAscertainment { get; set; }
    }
}