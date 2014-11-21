using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositions
{
    // TODO {all, 09.07.2014}: посмотреть есть ли необходимость в таком operationservice или нет (один из подозрительных моментов - отсутствие логирования операции), весь его функционал впишется в одну из более подходящих readmodel, например, в price, также, возможно, стоит избавиться от сигнатур с orderId, предоставив вызывающему коду самому делать resolve orderid->firmid
    public class CalculateCategoryRateOperationService : ICalculateCategoryRateOperationService
    {
        private const decimal DefaultCategoryRate = 1m;

        private static readonly Func<decimal> DefaultRateStrategy = () => DefaultCategoryRate;
        private static readonly Func<decimal> ExceptionStrategy = () =>
            {
                throw new NotificationException(BLResources.CategoryShouldBeSpecifiedForTheBoundCategoryRateType);
            };

        private readonly IPriceReadModel _priceReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public CalculateCategoryRateOperationService(IPriceReadModel priceReadModel, IFirmReadModel firmReadModel)
        {
            _priceReadModel = priceReadModel;
            _firmReadModel = firmReadModel;
        }

        public decimal GetCategoryRateForOrderCalculated(long orderId, long pricePositionId, long[] categoryIds)
        {
            var firmId = _firmReadModel.GetOrderFirmId(orderId);
            return CalculateCategoryRate(firmId, pricePositionId, categoryIds, ExceptionStrategy);
        }

        public decimal GetCategoryRateForOrderCalculatedOrDefault(long orderId, long pricePositionId, long[] categoryIds)
        {
            var firmId = _firmReadModel.GetOrderFirmId(orderId);
            return CalculateCategoryRate(firmId, pricePositionId, categoryIds, DefaultRateStrategy);
        }

        public decimal GetCategoryRateForFirmCalculated(long? firmId, long pricePositionId, long[] categoryIds)
        {
            return firmId.HasValue
                       ? CalculateCategoryRate(firmId.Value, pricePositionId, categoryIds, ExceptionStrategy)
                       : DefaultCategoryRate;
        }

        private decimal CalculateCategoryRate(long firmId, long pricePositionId, long[] categoryIds, Func<decimal> missingCategoryIdsStrategy)
        {
            var rateType = _priceReadModel.GetPricePositionRateType(pricePositionId);
            switch (rateType)
            {
                case PricePositionRateType.None:
                    return DefaultCategoryRate;
                case PricePositionRateType.MostExpensiveCategory:
                    return _priceReadModel.GetCategoryRateByFirm(firmId);
                case PricePositionRateType.BoundCategory:
                    if (categoryIds == null || !categoryIds.Any())
                    {
                        return missingCategoryIdsStrategy.Invoke();
                    }

                    var organizationUnitId = _firmReadModel.GetOrgUnitId(firmId);
                    return _priceReadModel.GetCategoryRateByCategory(categoryIds, organizationUnitId);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
