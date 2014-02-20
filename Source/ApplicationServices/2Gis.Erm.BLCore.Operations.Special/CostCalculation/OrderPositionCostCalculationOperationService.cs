﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Special.CostCalculation
{
    public class OrderPositionCostCalculationOperationService : ICalculateOrderPositionCostService
    {
        private readonly IFinder _finder;
        private readonly ICostCalculator _costCalculator;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IFirmRepository _firmRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IProjectService _projectService;

        public OrderPositionCostCalculationOperationService(
            IFinder finder,
            ICostCalculator costCalculator,
            IClientProxyFactory clientProxyFactory,
                                                            IOrderReadModel orderReadModel,
            IFirmRepository firmRepository,
            IOperationScopeFactory scopeFactory,
                                                            IProjectService projectService,
                                                            IPriceReadModel priceReadModel)
        {
            _finder = finder;
            _costCalculator = costCalculator;
            _clientProxyFactory = clientProxyFactory;
            _orderReadModel = orderReadModel;
            _firmRepository = firmRepository;
            _scopeFactory = scopeFactory;
            _projectService = projectService;
            _priceReadModel = priceReadModel;
        }

        /// <summary>
        /// Рассчитываем стоимость позиций заказа. Возвращает итоговые суммы и раложение по каждой позиции
        /// </summary>
        public CalculationResult CalculateOrderPositionsCostWithActualPrice(
            OrderType orderType,
            int orderReleaseCount,
            DateTime beginDistributionDate,
            long? sourceProjectCode,
            long destProjectCode,
            long? firmId,
            long? categoryId,
            IList<CalcPositionWithDiscountInfo> positionInfos)
        {
            long? sourceOrganizationUnitId = null;
            if (sourceProjectCode.HasValue)
            {
                var sourceProject = _projectService.GetProjectByCode(sourceProjectCode.Value);
                if (!sourceProject.OrganizationUnitId.HasValue)
                {
                    throw new InvalidOperationException(string.Format(BLResources.ProjectHasNoOrganizationUnit, sourceProject.DisplayName));
                }

                sourceOrganizationUnitId = sourceProject.OrganizationUnitId.Value;
            }

            var destProject = _projectService.GetProjectByCode(destProjectCode);
            if (!destProject.OrganizationUnitId.HasValue)
            {
                throw new InvalidOperationException(string.Format(BLResources.ProjectHasNoOrganizationUnit, destProject.DisplayName));
            }

            var destOrganizationUnitId = destProject.OrganizationUnitId.Value;

            long priceId;
            if (!_orderReadModel.TryGetActualPriceId(destOrganizationUnitId, beginDistributionDate, out priceId))
            {
                throw new InvalidOperationException(string.Format(BLResources.CannotGetActualPriceListForOrganizationUnit, destOrganizationUnitId));
            }

            return CalculateOrderPositionsCost(orderType,
                                               orderReleaseCount,
                                               priceId,
                                               sourceOrganizationUnitId,
                                               destOrganizationUnitId,
                                               firmId,
                                               categoryId,
                                               positionInfos);
        }

        public CalculationResult CalculateOrderPositionsCostWithActualPrice(OrderType orderType,
                                                                            int orderReleaseCount,
                                                                            DateTime beginDistributionDate,
                                                                            long? sourceProjectCode,
                                                                            long firmId,
                                                                            long? categoryId,
                                                                            IList<CalcPositionWithDiscountInfo> positionInfos)
        {
            var firm = _firmRepository.GetFirm(firmId);
            if (firm == null)
            {
                throw new InvalidOperationException(string.Format("Фирма с идентификатором {0} не найдена", firmId));
            }

            long? sourceOrganizationUnitId = null;
            if (sourceProjectCode.HasValue)
            {
                var sourceProject = _projectService.GetProjectByCode(sourceProjectCode.Value);
                if (!sourceProject.OrganizationUnitId.HasValue)
                {
                    throw new InvalidOperationException(string.Format(BLResources.ProjectHasNoOrganizationUnit, sourceProject.DisplayName));
                }

                sourceOrganizationUnitId = sourceProject.OrganizationUnitId.Value;
            }

            var destOrganizationUnitId = firm.OrganizationUnitId;

            long priceId;
            if (!_orderReadModel.TryGetActualPriceId(destOrganizationUnitId, beginDistributionDate, out priceId))
            {
                throw new InvalidOperationException(string.Format(BLResources.CannotGetActualPriceListForOrganizationUnit, destOrganizationUnitId));
            }

            return CalculateOrderPositionsCost(orderType,
                                               orderReleaseCount,
                                               priceId,
                                               sourceOrganizationUnitId,
                                               destOrganizationUnitId,
                                               firmId,
                                               categoryId,
                                               positionInfos);
        }

        /// <summary>
        /// Рассчитываем стоимость позиций заказа. Возвращает итоговые суммы и раложение по каждой позиции
        /// </summary>
        public CalculationResult CalculateOrderPositionsCost(
            OrderType orderType,
            int orderReleaseCount,
            long priceId,
            long? sourceOrganizationUnitId,
            long destOrganizationUnitId,
            long? firmId,
            long? categoryId,
            IList<CalcPositionWithDiscountInfo> positionInfos)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<CalculateOrderCostIdentity>())
            {
                bool showVat;
                var vatRate = _orderReadModel.GetVatRate(sourceOrganizationUnitId, destOrganizationUnitId, out showVat);

                var positionCalcs = new CalculationResult[positionInfos.Count()];

                for (var i = 0; i < positionInfos.Count(); i++)
                {
                    positionCalcs[i] = CalculateOrderPositionCost(orderType,
                                                                  orderReleaseCount,
                                                                  firmId,
                                                                  categoryId,
                                                                  positionInfos[i].PositionInfo.PositionId,
                                                                  priceId,
                                                                  positionInfos[i].PositionInfo.Amount,
                                                                  vatRate,
                                                                  showVat,
                                                                  positionInfos[i].DiscountInfo.Sum,
                                                                  positionInfos[i].DiscountInfo.Percent,
                                                                  positionInfos[i].DiscountInfo.CalculateDiscountViaPercent);
                }

                var result = _costCalculator.GetTotalResult(positionCalcs);
                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// Рассчитываем стоимость позиции заказа. Возвращает итоговую сумму и раложение по каждой подпозиции
        /// </summary>
        public CalculationResult CalculateOrderPositionCost(
            OrderType orderType,
            int orderReleaseCount,
            long priceId,
            long? sourceOrganizationUnitId,
            long destOrganizationUnitId,
            long? firmId,
            long? categoryId,
            CalcPositionWithDiscountInfo positionInfo)
        {
            // рассчитаем результат для коллекции из одной позиции
            var result = CalculateOrderPositionsCost(orderType,
                                                     orderReleaseCount,
                                                     priceId,
                                                     sourceOrganizationUnitId,
                                                     destOrganizationUnitId,
                                                     firmId,
                                                     categoryId,
                                                     new List<CalcPositionWithDiscountInfo> { positionInfo });

            return result.PositionCalcs.Single();
        }

        /// <summary>
        /// Рассчитывает стоимость отдельной позиции заказа
        /// </summary>
        public CalculationResult CalculateOrderPositionCost(
            OrderType orderType,
            int orderReleaseCount,
            long? firmId,
            long? categoryId,
            long positionId,
            long priceId,
            int amount,
            decimal vatRate,
            bool showVat,
            decimal discountSum,
            decimal discountPercent,
            bool calculateDiscountViaPercent)
        {
            var pricePositionInfo = _finder.Find(
                PricePositionSpecifications.Find.ByPriceAndPostion(positionId, priceId) &
                Specs.Find.ActiveAndNotDeleted<PricePosition>())
                                           .Select(
                                               x =>
                                               new
                                               {
                                                   x.Id,
                                                   x.Cost,
                                                   x.RateType,
                                                   x.Position.IsComposite,
                                               })
                                           .SingleOrDefault();

            if (pricePositionInfo == null)
            {
                // Такой позиции прайса нет, значит выбранную позицию нельзя приобрести в городе размещения
                var positionName = _finder.Find(Specs.Find.ById<Position>(positionId)).Select(x => x.Name).Single();

                throw new PositionIsNotRepresentedException(string.Format(BLResources.PositionIsNotInPrice, positionName));
            }

            var nullCostOrder = orderType == OrderType.SelfAds || orderType == OrderType.SocialAds;

            var categoryRate = firmId.HasValue ? _priceReadModel.GetCategoryRate(pricePositionInfo.Id, firmId.Value, categoryId) : 1m;

            if (pricePositionInfo.IsComposite)
            {
                var clientProxy =
                    _clientProxyFactory.GetClientProxy<IWithdrawalInfoApplicationService, WSHttpBinding>();
                var positionCosts =
                    clientProxy.Execute(action => action.GetPriceCostsForSubPositions(positionId, priceId));

                if (positionCosts.Any())
                {
                    var subpositionCalcRequests = positionCosts.Select(x => new CalcPositionRequest
                    {
                        Amount = amount,
                        PriceCost = nullCostOrder ? decimal.Zero : x.Cost,
                        Rate = categoryRate,
                        PositionId = x.PositionId,
                        PriceId = priceId,
                        ReleaseCount = orderReleaseCount,
                        ShowVat = showVat,
                        VatRate = vatRate,

                        // Заполнять информацию о скидках не имеет смысла т.к, будет использована скидка пакета
                    }).ToArray();

                    var packageCalculation = CalculateWithDiscountDistribution(discountSum,
                                                                               discountPercent,
                                                                               calculateDiscountViaPercent,
                                                                               subpositionCalcRequests);
                    packageCalculation.PositionId = positionId;
                    packageCalculation.PriceId = priceId;
                    return packageCalculation;
                }
            }

            return _costCalculator.Calculate(new CalcPositionRequest
            {
                Amount = amount,
                CalculateDiscountViaPercent = calculateDiscountViaPercent,
                DiscountPercent = discountPercent,
                DiscountSum = discountSum,
                PriceCost = nullCostOrder ? decimal.Zero : pricePositionInfo.Cost,
                Rate = categoryRate,
                PositionId = positionId,
                PriceId = priceId,
                ReleaseCount = orderReleaseCount,
                ShowVat = showVat,
                VatRate = vatRate
            });
        }

        /// <summary>
        /// Делает расчет заказа или пакета с учетом распределения скидки
        /// </summary>
        private CalculationResult CalculateWithDiscountDistribution(
            decimal discountSum,
            decimal discountPercent,
            bool calculateDiscountViaPercent,
            IList<CalcPositionRequest> positionInfos)
        {
            // сначала запустим расчет без учета скидок, получим итоговую PayablePrice и актуализируем скидки
            var subPositionCalcs = positionInfos.Select(_costCalculator.Calculate).ToList();

            // теперь уточним значения скидки
            var totalPayablePrice = subPositionCalcs.Sum(x => x.PayablePriceWithVat);
            var actualDiscount = _costCalculator.CalculateDiscount(totalPayablePrice, discountSum, discountPercent, calculateDiscountViaPercent);

            // Пересчитаем показатели с учетом уточненных скидок. Позиции считаем со скидкой в процентах
            var totalDiscountSum = 0M;
            subPositionCalcs.Clear();
            for (var i = 0; i < positionInfos.Count - 1; i++)
            {
                var request = positionInfos[i];
                request.CalculateDiscountViaPercent = true;
                request.DiscountPercent = actualDiscount.Percent;
                var response = _costCalculator.Calculate(request);
                subPositionCalcs.Add(response);

                totalDiscountSum += response.DiscountSum;
            }

            // последнюю позицию надо рассчитать не в процентах, чтобы сохранить точность
            var lastRequest = positionInfos.Last();
            lastRequest.CalculateDiscountViaPercent = false;
            lastRequest.DiscountSum = actualDiscount.Sum - totalDiscountSum;
            subPositionCalcs.Add(_costCalculator.Calculate(lastRequest));

            // Все позиции расчитаны. Теперь можно получить суммы для заказа или пакета.
            return _costCalculator.GetTotalResult(subPositionCalcs);
        }
    }
}
