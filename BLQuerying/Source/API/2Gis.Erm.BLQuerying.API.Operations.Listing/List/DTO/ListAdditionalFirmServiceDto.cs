using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdditionalFirmServiceDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string ServiceCode { get; set; }
        public string Description { get; set; }
        public bool IsManaged { get; set; }
    }
}