using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListCountryDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string IsoCode { get; set; }
        public string Name { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public bool IsDeleted { get; set; }
    }
}