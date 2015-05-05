using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto
{
    public sealed class PricePositionDetailedInfo
    {
        public long PositionId { get; set; }
        public decimal PricePositionCost { get; set; }
        public int AmountSpecificationMode { get; set; }
        public int? Amount { get; set; }
        public string Platform { get; set; }
        public PricePositionRateType RateType { get; set; }
        public bool IsComposite { get; set; }
        public PositionBindingObjectType BindingObjectType { get; set; }
        public SalesModel SalesModel { get; set; }
    }
}