using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    public interface ICostCalculator
    {
        /// <summary>
        /// Подсчитывает результат заказа или пакета по расчитанным позициям
        /// </summary>
        CalculationResult GetTotalResult(IList<CalculationResult> positionResults);

        DiscountInfo CalculateDiscount(decimal price, decimal discountSum, decimal discountPercent, bool calculateDiscountViaPercent);

        /// <summary>
        /// Непосредственно расчет стоимости простой позиции
        /// </summary>
        CalculationResult Calculate(CalcPositionRequest positionInfo);

        PricePerUnitCalculationResult CalculatePricePerUnit(decimal priceCost, decimal categoryRate, decimal vatRate, bool showVat);
    }
}