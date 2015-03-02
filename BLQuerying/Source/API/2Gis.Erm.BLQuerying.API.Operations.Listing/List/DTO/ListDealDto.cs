using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListDealDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public long? MainFirmId { get; set; }
        public string MainFirmName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long OwnerCode { get; set; }
        public bool IsOwner { get; set; }
    }
}