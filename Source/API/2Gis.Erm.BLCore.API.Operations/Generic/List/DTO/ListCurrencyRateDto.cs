using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListCurrencyRateDto : IListItemEntityDto<CurrencyRate>
    {
        public long Id { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public long BaseCurrencyId { get; set; }
        public string BaseCurrencyName { get; set; }
        public decimal Rate { get; set; }
        public DateTime CreatedOn { get; set; }
        public long OwnerCode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCurrent { get; set; }
    }
}