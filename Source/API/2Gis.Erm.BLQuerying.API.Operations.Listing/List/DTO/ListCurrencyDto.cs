using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListCurrencyDto : IListItemEntityDto<Currency>
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