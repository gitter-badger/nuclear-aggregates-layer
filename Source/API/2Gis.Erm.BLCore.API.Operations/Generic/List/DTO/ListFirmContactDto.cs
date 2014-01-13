using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListFirmContactDto : IListItemEntityDto<FirmContact>
    {
        public long Id { get; set; }
        public string ContactType { get; set; }
        public string Contact { get; set; }
    }
}