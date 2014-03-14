using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;

namespace DoubleGis.Erm.BL.Operations.Special.CostCalculation
{
    public sealed class CostCalculator : ICostCalculator
    {
        private readonly IBusinessModelSettings _businessModelSettings;

        public CostCalculator(IBusinessModelSettings businessModelSettings)
        {
            _businessModelSettings = businessModelSettings;
        }

        /// <summary>
        /// Подсчитывает результат заказа или пакета по расчитанным позициям
        /// </summary>
        public CalculationResult GetTotalResult(IList<CalculationResult> positionResults)
        {
            if (!(positionResults.All(x => x.Vat.HasValue) || positionResults.All(x => !x.Vat.HasValue)))
            {
                throw new InvalidOperationException("У всех позиций пакета или заказа должно быть одно значение свойства 'ShowVat'");
            }

            var showVat = positionResults.All(x => x.Vat.HasValue);

            var result = new CalculationResult
            {
                PositionCalcs = positionResults,
                Vat = showVat ? 0M : (decimal?)null
            };

            foreach (var positionCalc in positionResults)
            {
                result.PayablePlan += positionCalc.PayablePlan;
                result.PayablePlanWoVat += positionCalc.PayablePlanWoVat;
                result.DiscountSum += positionCalc.DiscountSum;
                result.PayablePriceWithVat += positionCalc.PayablePriceWithVat;
                result.PayablePriceWithoutVat += positionCalc.PayablePriceWithoutVat;
                if (showVat)
                {
                    result.Vat += positionCalc.Vat;
                }

                // Имеет смысл только для пакета
                result.PricePerUnit += positionCalc.PricePerUnit;
                result.PricePerUnitWithVat += positionCalc.PricePerUnitWithVat;
            }

            // у пакета коэфициент группы рубрик всех подпозиций будет совпадать
            var positionUniqueRates = positionResults.Select(x => x.Rate).Distinct().ToArray();
            if (positionUniqueRates.Count() == 1)
            {
                result.Rate = positionUniqueRates.Single();
            }

            // актуализируем процент скидки
            var actualDiscount = CalculateDiscount(result.PayablePriceWithVat, result.DiscountSum, 0M, false);
            result.DiscountSum = actualDiscount.Sum;
            result.DiscountPercent = actualDiscount.Percent;

            return result;
        }

        public DiscountInfo CalculateDiscount(decimal price, decimal discountSum, decimal discountPercent, bool calculateDiscountViaPercent)
        {
            var result = new DiscountInfo
            {
                CalculateDiscountViaPercent = calculateDiscountViaPercent
            };

            if (price == decimal.Zero)
            {
                result.Percent = decimal.Zero;
                result.Sum = decimal.Zero;

                return result;
            }

            var exactDiscountSum = calculateDiscountViaPercent ? (price * discountPercent) / 100m : Math.Min(price, discountSum);
            result.Sum = Math.Round(exactDiscountSum, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);

            result.Percent = (exactDiscountSum * 100m) / price;

            return result;
        }

        /// <summary>
        /// Непосредственно расчет стоимости простой позиции
        /// </summary>
        public CalculationResult Calculate(CalcPositionRequest positionInfo)
        {
            decimal? vat = null;

            var shipmentPlan = positionInfo.ReleaseCount * positionInfo.Amount;

            var pricePerUnitWithVat = positionInfo.PriceCost * positionInfo.Rate * (decimal.One + (positionInfo.VatRate / 100));
            pricePerUnitWithVat = Math.Round(pricePerUnitWithVat, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);

            // Иногда (Франчайзи-Филиал) цена без НДС включает в себя НДС
            var pricePerUnitWithoutVat = pricePerUnitWithVat;
            if (positionInfo.ShowVat)
            {
                pricePerUnitWithoutVat = positionInfo.PriceCost * positionInfo.Rate;
                pricePerUnitWithoutVat = Math.Round(pricePerUnitWithoutVat, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
            }

            var payablePriceWithVat = pricePerUnitWithVat * shipmentPlan;
            payablePriceWithVat = Math.Round(payablePriceWithVat, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);

            // Пользователю в качестве прайсовой цены мы показываем именно эту, т.е. без НДС
            var payablePriceWithoutVat = pricePerUnitWithoutVat * shipmentPlan;
            payablePriceWithoutVat = Math.Round(payablePriceWithoutVat, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);

            var discount = CalculateDiscount(payablePriceWithVat, positionInfo.DiscountSum, positionInfo.DiscountPercent, positionInfo.CalculateDiscountViaPercent);

            var payablePlanWithVat = payablePriceWithVat - discount.Sum;
            payablePlanWithVat = Math.Round(payablePlanWithVat, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);

            var payablePlanWithoutVat = (pricePerUnitWithoutVat != 0m)
                                       ? payablePlanWithVat / (pricePerUnitWithVat / pricePerUnitWithoutVat)
                                       : 0m;

            payablePlanWithoutVat = Math.Round(payablePlanWithoutVat, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);

            if (positionInfo.ShowVat)
            {
                vat = payablePlanWithVat - payablePlanWithoutVat;
            }

            return new CalculationResult
            {
                PositionId = positionInfo.PositionId,
                PriceId = positionInfo.PriceId,
                ShipmentPlan = shipmentPlan,
                DiscountSum = discount.Sum,
                DiscountPercent = discount.Percent,
                PayablePlan = payablePlanWithVat,
                PayablePlanWoVat = payablePlanWithoutVat,
                PayablePriceWithoutVat = payablePriceWithoutVat,
                PayablePriceWithVat = payablePriceWithVat,
                PricePerUnit = pricePerUnitWithoutVat,
                PricePerUnitWithVat = pricePerUnitWithVat,
                Rate = positionInfo.Rate,
                Vat = vat
            };
        }

        public decimal RoundValueToSignificantDigits(decimal value)
        {
            // TODO {y.baranihin, 05.02.2014}: Перенести форматирование денег на клиент
            // если внезапно за значимыми знаками стоят не 0, то округлять не будем
            var x = value * (int)Math.Pow(10, _businessModelSettings.SignificantDigitsNumber);
            if ((x - (int)x) != 0)
            {
                return value;
            }

            return Math.Round(value, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
        }
    }
}
