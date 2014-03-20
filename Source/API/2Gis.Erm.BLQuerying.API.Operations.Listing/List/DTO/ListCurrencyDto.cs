using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListCurrencyDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long ISOCode { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public bool IsBase { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}