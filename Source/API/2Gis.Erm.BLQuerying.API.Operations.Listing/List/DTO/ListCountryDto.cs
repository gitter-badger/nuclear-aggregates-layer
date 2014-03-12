using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListCountryDto : IListItemEntityDto<Country>
    {
        public long Id { get; set; }
        public string IsoCode { get; set; }
        public string Name { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public bool IsDeleted { get; set; }
    }
}