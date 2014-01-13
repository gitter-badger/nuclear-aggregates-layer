using System;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies
{
    public sealed class CurrencyWithRelationsDto
    {
        public Platform.Model.Entities.Erm.Currency Currency { get; set; }
        public string RelatedCountryName { get; set; }
        public DateTime? RelatedCurrencyRateCreateDate { get; set; }
    }
}