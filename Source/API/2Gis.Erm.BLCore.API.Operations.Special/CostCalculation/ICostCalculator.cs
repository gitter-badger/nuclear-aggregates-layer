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

        // TODO {d.ivanov, 03.02.2014}: Есть вопросы к этому методу: должен ли он быть экземплярными, объявлен ли он в нужном класса?
        /// <summary>
        /// Окруление некоторой суммы денег до значимых цифр после зяпятой
        /// </summary>
        decimal RoundValueToSignificantDigits(decimal value);
    }
}