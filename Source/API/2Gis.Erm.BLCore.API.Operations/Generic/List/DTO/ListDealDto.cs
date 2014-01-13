using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListDealDto : IListItemEntityDto<Deal>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public long? MainFirmId { get; set; }
        public string MainFirmName { get; set; }
    }
}