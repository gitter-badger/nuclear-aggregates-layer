namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation
{
    public sealed class ValidResult
    {
        public long OrderId { get; set; }
        public ValidationContext ValidationContext { get; set; }
    }
}