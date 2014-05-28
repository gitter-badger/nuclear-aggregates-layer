namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    public class CalcPositionWithDiscountInfo
    {
        public ICalcPositionInfo PositionInfo { get; set; }
        public DiscountInfo DiscountInfo { get; set; }
        public decimal CategoryRate { get; set; }
    }
}
