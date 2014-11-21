namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    public interface ICostCalculationResult
    {
        string ResultType { get; set; }

        long PositionId { get; set; }
    }
}
