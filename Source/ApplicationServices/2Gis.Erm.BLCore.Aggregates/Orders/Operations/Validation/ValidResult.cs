namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Validation
{
    internal sealed class ValidResult
    {
        public long OrderId { get; set; }
        public ValidationContext ValidationContext { get; set; }
    }
}