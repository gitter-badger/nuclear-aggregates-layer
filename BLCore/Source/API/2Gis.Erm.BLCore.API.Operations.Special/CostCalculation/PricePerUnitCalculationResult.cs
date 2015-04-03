namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    public sealed class PricePerUnitCalculationResult
    {
        /// <summary>
        /// Цена за единицу.
        /// </summary>
        public decimal PricePerUnit { get; set; }

        /// <summary>
        /// Цена за единицу с НДС.
        /// </summary>
        public decimal PricePerUnitWithVat { get; set; }
    }
}
