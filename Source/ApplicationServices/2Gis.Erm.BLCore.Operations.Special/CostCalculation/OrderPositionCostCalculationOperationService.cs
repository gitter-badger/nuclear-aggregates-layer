using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
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
        private const decimal DefaultVatRate = 18M;

        private readonly IFinder _finder;
        private readonly ICostCalculator _costCalculator;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IOrderRepository _orderRepository;
        private readonly IFirmRepository _firmRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IProjectService _projectService;

        public OrderPositionCostCalculationOperationService(
            IOrderRepository orderRepository,
            IFinder finder,
            ICostCalculator costCalculator,
            IClientProxyFactory clientProxyFactory,
            IFirmRepository firmRepository,
            IOperationScopeFactory scopeFactory,
            IProjectService projectService)
        {
            _orderRepository = orderRepository;
            _finder = finder;
            _costCalculator = costCalculator;
            _clientProxyFactory = clientProxyFactory;
            _firmRepository = firmRepository;
            _scopeFactory = scopeFactory;
            _projectService = projectService;
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
            if (!_orderRepository.TryGetActualPriceId(destOrganizationUnitId, beginDistributionDate, out priceId))
            {
                throw new InvalidOperationException(string.Format(BLResources.CannotGetActualPriceListForOrganizationUnit, destOrganizationUnitId));
            }

            return CalculateOrderPositionsCost(orderType,
                                               orderReleaseCount,
                                               priceId,
                                               sourceOrganizationUnitId,
                                               destOrganizationUnitId,
                                               firmId,
                                               positionInfos);
        }

        public CalculationResult CalculateOrderPositionsCostWithActualPrice(OrderType orderType,
                                                                            int orderReleaseCount,
                                                                            DateTime beginDistributionDate,
                                                                            long? sourceProjectCode,
                                                                            long firmId,
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
            if (!_orderRepository.TryGetActualPriceId(destOrganizationUnitId, beginDistributionDate, out priceId))
            {
                throw new InvalidOperationException(string.Format(BLResources.CannotGetActualPriceListForOrganizationUnit, destOrganizationUnitId));
            }

            return CalculateOrderPositionsCost(orderType,
                                               orderReleaseCount,
                                               priceId,
                                               sourceOrganizationUnitId,
                                               destOrganizationUnitId,
                                               firmId,
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
            IList<CalcPositionWithDiscountInfo> positionInfos)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<CalculateOrderCostIdentity>())
            {
                bool showVat;
                var vatRate = GetVatRate(sourceOrganizationUnitId, destOrganizationUnitId, out showVat);

                var positionCalcs = new CalculationResult[positionInfos.Count()];

                for (var i = 0; i < positionInfos.Count(); i++)
                {
                    positionCalcs[i] = CalculateOrderPositionCost(orderType,
                                                                  orderReleaseCount,
                                                                  firmId,
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
            CalcPositionWithDiscountInfo positionInfo)
        {
            // рассчитаем результат для коллекции из одной позиции
            var result = CalculateOrderPositionsCost(orderType,
                                                     orderReleaseCount,
                                                     priceId,
                                                     sourceOrganizationUnitId,
                                                     destOrganizationUnitId,
                                                     firmId,
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
                                                   x.Cost,
                                                   x.RatePricePositions,
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

            var currentRate = CategoryRate.Default;
            if (pricePositionInfo.RatePricePositions && firmId.HasValue)
            {
                currentRate = GetCategoryRate(firmId.Value);
            }

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
                        Rate = currentRate,
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
                Rate = currentRate,
                PositionId = positionId,
                PriceId = priceId,
                ReleaseCount = orderReleaseCount,
                ShowVat = showVat,
                VatRate = vatRate
            });
        }

        // TODO {y.baranihin, 18.11.2013}: вынести в read-model
        private CategoryRate GetCategoryRate(long firmId)
        {
            var organizationUnitId =
                _finder.Find(Specs.Find.ById<Firm>(firmId)).Select(x => x.OrganizationUnitId).Single();

            // Максимальный коэффициент по всем рубрикам этой фирмы в том отделении 2гис, где размещается заказ.
            const decimal DefaultCategoryRate = 1M;
            var categoryRates = _finder.Find(Specs.Find.ById<Firm>(firmId))
                                       .SelectMany(firm => firm.FirmAddresses)
                                       .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                       .SelectMany(address => address.CategoryFirmAddresses)
                                       .Where(Specs.Find.Active<CategoryFirmAddress>())
                                       .Select(addressCategory => addressCategory.Category)
                                       .Where(Specs.Find.ActiveAndNotDeleted<Category>())
                                       .SelectMany(category => category.CategoryOrganizationUnits)
                                       .Where(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>())
                                       .Where(categoryOrganizationUnit => categoryOrganizationUnit.OrganizationUnitId == organizationUnitId)
                                       .Select(
                                           categoryOrganizationUnit =>
                                           categoryOrganizationUnit.CategoryGroup != null
                                               ? categoryOrganizationUnit.CategoryGroup.GroupRate
                                               : DefaultCategoryRate)
                                       .ToArray();

            if (!categoryRates.Any())
            {
                throw new BusinessLogicException(BLResources.PricePositionCannotBeChoosedSinceThereIsNoFirmCategory);
            }

            return CategoryRate.Known(categoryRates.Max());
        }

        /// <summary>
        /// Получаем значение процента НДС и информацию о том, отображается ли он в документах
        /// </summary>
        // TODO {y.baranihin, 18.11.2013}: вынести в read-model
        private decimal GetVatRate(long? sourceOrganizationUnitId, long destOrganizationUnitId, out bool showVat)
        {
            var sourceVat = DefaultVatRate;
            if (sourceOrganizationUnitId.HasValue)
            {
                sourceVat = _finder.Find<OrganizationUnit, decimal>(OrganizationUnitSpecs.Select.VatRate(),
                                                                    Specs.Find.ById<OrganizationUnit>(sourceOrganizationUnitId.Value))
                                   .Single();
            }

            var destVat = _finder.Find<OrganizationUnit, decimal>(OrganizationUnitSpecs.Select.VatRate(),
                                                                  Specs.Find.ById<OrganizationUnit>(destOrganizationUnitId))
                                 .Single();

            if (sourceVat == decimal.Zero)
            {
                // Город источник - франчайзи
                showVat = false;
                return destVat;
            }

            // Город источник - филиал
            showVat = true;
            return sourceVat;
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
