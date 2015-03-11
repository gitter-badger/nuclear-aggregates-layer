namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto
{
    public sealed class OrderPositionInfoDto
    {
        public decimal AmountToWithdraw { get; set; }
        public long PriceId { get; set; }
        public decimal CategoryRate { get; set; }
        public decimal DiscountPercent { get; set; }
        public long OrderPositionId { get; set; }
    }
}