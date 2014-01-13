using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListFirmAddressDto : IListItemEntityDto<FirmAddress>
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public string FirmName { get; set; }
        public string Address { get; set; }
    }
}