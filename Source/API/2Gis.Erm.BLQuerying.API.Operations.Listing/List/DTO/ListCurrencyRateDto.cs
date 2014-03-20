using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListCurrencyRateDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public long BaseCurrencyId { get; set; }
        public string BaseCurrencyName { get; set; }
        public decimal Rate { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCurrent { get; set; }
    }
}