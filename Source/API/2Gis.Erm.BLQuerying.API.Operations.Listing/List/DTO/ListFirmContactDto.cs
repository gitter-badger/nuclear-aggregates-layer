using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListFirmContactDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public FirmAddressContactType ContactTypeEnum { get; set; }
        public string ContactType { get; set; }
        public string Contact { get; set; }
        public long? CardId { get; set; }
        public long? FirmAddressId { get; set; }
    }
}