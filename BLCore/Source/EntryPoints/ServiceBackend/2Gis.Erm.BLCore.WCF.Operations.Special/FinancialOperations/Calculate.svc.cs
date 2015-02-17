using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.CostCalculation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.WCF.Operations.Special.FinancialOperations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class CostCalculationApplicationService : ICostCalculationApplicationService
    {
        private readonly ICalculateOrderCostService _calculateOrderCostService;
        private readonly ICalculateOrderPositionCostService _calculateOrderPositionCostService;
        private readonly ICommonLog _logger;
        private readonly IGetPositionsByOrderService _getPositionsByOrderService;

        public CostCalculationApplicationService(IUserContext userContext,
                                                 ICalculateOrderCostService calculateOrderCostService,
                                                 ICommonLog logger,
                                                 IGetPositionsByOrderService getPositionsByOrderService,
                                                 ICalculateOrderPositionCostService calculateOrderPositionCostService,
                                                 IResourceGroupManager resourceGroupManager)
        {
            _calculateOrderCostService = calculateOrderCostService;
            _logger = logger;
            _getPositionsByOrderService = getPositionsByOrderService;
            _calculateOrderPositionCostService = calculateOrderPositionCostService;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public ICostCalculationResult[] CalculateOrderProlongation(long orderId)
        {
            try
            {
                var calculationResult = _calculateOrderCostService.CalculateOrderProlongation(orderId);

                return TransformResultForGenericClient(calculationResult);
            }
            catch (PositionIsNotRepresentedException exception)
            {
                _logger.ErrorFormat(exception, "Error has occured in {0}", GetType().Name);
                return GetEmptyCalculationResults(orderId);
            }
            catch (BusinessLogicException exception)
            {
                _logger.ErrorFormat(exception, "Error has occured in {0}", GetType().Name);
                throw new FaultException<CostCalculatonErrorDescription>(new CostCalculatonErrorDescription(exception.Message));
            }
            catch (Exception exception)
            {
                _logger.FatalFormat(exception, "Error has occured in {0}", GetType().Name);
                throw new FaultException<CostCalculatonErrorDescription>(new CostCalculatonErrorDescription(BLResources.InTheCostCalculationServiceErrorOccured));
            }
        }

        public ICostCalculationResult[] CalculateOrderCostWithFullData(DateTime beginDistributionDate,
                                                                       long sourceProjectCode,
                                                                       long firmId,
                                                                       IEnumerable<ICalcPositionInfo> positionsInfo)
        {
            try
            {
                var positionsWithDiscounts = positionsInfo
                    .Select(x => new CalcPositionWithDiscountInfo
                        {
                            DiscountInfo = new DiscountInfo(),
                            PositionInfo = x
                        })
                    .ToArray();

                var calculationResult = _calculateOrderPositionCostService.CalculateOrderPositionsCostWithActualPrice(OrderType.Sale,
                                                                                                                      1,
                                                                                                                      beginDistributionDate,
                                                                                                                      sourceProjectCode,
                                                                                                                      firmId,
                                                                                                                      null,
                                                                                                                      positionsWithDiscounts);

                return TransformResultForGenericClient(calculationResult);
            }
            catch (BusinessLogicException exception)
            {
                _logger.ErrorFormat(exception, "Error has occured in {0}", GetType().Name);
                throw new FaultException<CostCalculatonErrorDescription>(new CostCalculatonErrorDescription(exception.Message));
            }
            catch (Exception exception)
            {
                _logger.FatalFormat(exception, "Error has occured in {0}", GetType().Name);
                throw new FaultException<CostCalculatonErrorDescription>(new CostCalculatonErrorDescription(BLResources.InTheCostCalculationServiceErrorOccured));
            }
        }

        public ICostCalculationResult[] CalculateOrderCostWithShortcutData(long destProjectCode,
                                                                           IEnumerable<ICalcPositionInfo> positionsInfo)
        {
            try
            {
                var nextMonth = DateTime.Today.AddMonths(1);
                var beginDistributionDate = new DateTime(nextMonth.Year, nextMonth.Month, nextMonth.Day);

                var positionsWithDiscounts = positionsInfo
                    .Select(x => new CalcPositionWithDiscountInfo
                        {
                            DiscountInfo = new DiscountInfo(),
                            PositionInfo = x
                        })
                    .ToArray();

                var calculationResult = _calculateOrderPositionCostService.CalculateOrderPositionsCostWithActualPrice(OrderType.Sale,
                                                                                                                      1,
                                                                                                                      beginDistributionDate,
                                                                                                                      null,
                                                                                                                      destProjectCode,
                                                                                                                      null,
                                                                                                                      null,
                                                                                                                      positionsWithDiscounts);
                return TransformResultForGenericClient(calculationResult);
            }
            catch (BusinessLogicException exception)
            {
                _logger.ErrorFormat(exception, "Error has occured in {0}", GetType().Name);
                throw new FaultException<CostCalculatonErrorDescription>(new CostCalculatonErrorDescription(exception.Message));
            }
            catch (Exception exception)
            {
                _logger.FatalFormat(exception, "Error has occured in {0}", GetType().Name);
                throw new FaultException<CostCalculatonErrorDescription>(new CostCalculatonErrorDescription(BLResources.InTheCostCalculationServiceErrorOccured));
            }
        }

        private static ICostCalculationResult[] TransformResultForGenericClient(CalculationResult calculationResult)
        {
            if (!calculationResult.PositionCalcs.All(x => x.Vat.HasValue) &&
                calculationResult.PositionCalcs.Any(x => x.Vat.HasValue))
            {
                throw new InvalidOperationException("Для всех результатов НДС должен быть задан или отсутсвовать");
            }

            var result = calculationResult.PositionCalcs.Select(x => x.Vat.HasValue
                                                                         ? (ICostCalculationResult)new BizAccountPositionCostWithVatResult
                                                                                                       {
                                                                                                           PayablePlan = x.PayablePlan,
                                                                                                           PositionId = x.PositionId,
                                                                                                           DiscountSum = x.DiscountSum,
                                                                                                           Vat = x.Vat.Value
                                                                                                       }
                                                                         : (ICostCalculationResult)new BizAccountPositionCostResult
                                                                                                       {
                                                                                                           PayablePlan = x.PayablePlan,
                                                                                                           PositionId = x.PositionId,
                                                                                                           DiscountSum = x.DiscountSum,
                                                                                                       }).ToArray();

            return result;
        }

        private ICostCalculationResult[] GetEmptyCalculationResults(long orderId)
        {
            try
            {
                var positions = _getPositionsByOrderService.GetPositionIds(orderId);
                return positions
                    .Select(x => (ICostCalculationResult)new BizAccountEmptyCostResult
                        {
                            PositionId = x
                        }).ToArray();
            }
            catch (BusinessLogicException exception)
            {
                _logger.ErrorFormat(exception, "Error has occured in {0}", GetType().Name);
                throw new FaultException<CostCalculatonErrorDescription>(new CostCalculatonErrorDescription(exception.Message));
            }
            catch (Exception exception)
            {
                _logger.FatalFormat(exception, "Error has occured in {0}", GetType().Name);
                throw new FaultException<CostCalculatonErrorDescription>(new CostCalculatonErrorDescription(BLResources.InTheCostCalculationServiceErrorOccured));
            }
        }
    }
}
