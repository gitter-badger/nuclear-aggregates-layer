namespace DoubleGis.Erm.Core.Dto.DomainEntity.Custom
{
    public sealed class OrderPositionPriceDto
    {
        public bool DiscountInPercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountUnits { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal PricePerUnitVatInclude { get; set; }
        public int UnitCount { get; set; }
        public int ShipmentPlan { get; set; }
        public decimal PayablePrice { get; set; }
        public int PayablePlan { get; set; }
    }
}
