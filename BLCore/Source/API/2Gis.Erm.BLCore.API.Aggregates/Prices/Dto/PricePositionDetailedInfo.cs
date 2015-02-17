using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class PricePositionDetailedInfo
    {
        public decimal PricePositionCost { get; set; }
        public int AmountSpecificationMode { get; set; }
        public int? Amount { get; set; }
        public string Platform { get; set; }
        public bool IsComposite { get; set; }
        public PricePositionRateType RateType { get; set; }
        public PositionBindingObjectType LinkingObjectType { get; set; }
        public SalesModel SalesModel { get; set; }
        public long PositionId { get; set; }
    }
}