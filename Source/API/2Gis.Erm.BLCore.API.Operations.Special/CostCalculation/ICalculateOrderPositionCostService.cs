using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    public interface ICalculateOrderPositionCostService : IOperation<CalculateOrderPositionCostIdentity>
    {
        /// <summary>
        /// Рассчитывает стоимость отдельной позиции заказа
        /// </summary>
        CalculationResult CalculateOrderPositionCost(
            OrderType orderType,
            int orderReleaseCount,
            long? firmId,
            long positionId,
            long priceId,
            int amount,
            decimal vatRate,
            bool showVat,
            decimal discountSum,
            decimal discountPercent,
            bool calculateDiscountViaPercent);

        /// <summary>
        /// Рассчитываем стоимость позиций заказа. Возвращает итоговые суммы и раложение по каждой позиции
        /// </summary>
        CalculationResult CalculateOrderPositionsCostWithActualPrice(
            OrderType orderType,
            int orderReleaseCount,
            DateTime beginDistributionDate,
            long? sourceProjecId,
            long destProjectId,
            long? firmId,
            IList<CalcPositionWithDiscountInfo> positionInfos);

        /// <summary>
        /// Рассчитываем стоимость позиций заказа. Возвращает итоговые суммы и раложение по каждой позиции
        /// </summary>
        CalculationResult CalculateOrderPositionsCostWithActualPrice(
            OrderType orderType,
            int orderReleaseCount,
            DateTime beginDistributionDate,
            long? sourceProjecttId,
            long firmId,
            IList<CalcPositionWithDiscountInfo> positionInfos);

        /// <summary>
        /// Рассчитываем стоимость позиции заказа. Возвращает итоговую сумму и раложение по каждой подпозиции
        /// </summary>
        CalculationResult CalculateOrderPositionCost(
            OrderType orderType,
            int orderReleaseCount,
            long priceId,
            long? sourceOrganizationUnitId,
            long destOrganizationUnitId,
            long? firmId,
            CalcPositionWithDiscountInfo positionInfo);

        CalculationResult CalculateOrderPositionsCost(
            OrderType orderType,
            int orderReleaseCount,
            long priceId,
            long? sourceOrganizationUnitId,
            long destOrganizationUnitId,
            long? firmId,
            IList<CalcPositionWithDiscountInfo> positionInfos);
    }
}
