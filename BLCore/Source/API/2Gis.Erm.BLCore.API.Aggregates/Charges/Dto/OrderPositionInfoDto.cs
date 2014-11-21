namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto
{
    public sealed class OrderPositionInfoDto
    {
        public long PurchasedPositionId { get; set; }
        public decimal AmountToWithdraw { get; set; }
        public long PriceId { get; set; }
        public decimal CategoryRate { get; set; }
        public int Amount { get; set; }
        public decimal DiscountSum { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool CalculateDiscountViaPercent { get; set; }
        public long OrderPositionId { get; set; }
    }
}