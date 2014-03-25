using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositions
{
    // TODO {a.tukaev, 25.03.2014}: Лучше чтобы в назание абстракций и реализаций для OperationsService оканчивались именно на OperationService, т.к. сервисов разного вида у нас много 
    public class ViewOrderPositionOperationService : IViewOrderPositionOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly ICalculateCategoryRateOperationService _calculateCategoryRateOperationService;
        private readonly ISupportedCategoriesChecker _supportedCategoriesChecker;

        public ViewOrderPositionOperationService(IOrderReadModel orderReadModel,
                                        IPriceReadModel priceReadModel,
                                        ICalculateCategoryRateOperationService calculateCategoryRateOperationService,
                                        ISupportedCategoriesChecker supportedCategoriesChecker,
                                        IFirmReadModel firmReadModel)
        {
            _orderReadModel = orderReadModel;
            _priceReadModel = priceReadModel;
            _calculateCategoryRateOperationService = calculateCategoryRateOperationService;
            _supportedCategoriesChecker = supportedCategoriesChecker;
            _firmReadModel = firmReadModel;
        }

        // FIXME {all, 20.03.2014}: при рефакторинге ApplicationService (SRP, перевод на OperartionServices и т.п.) нужно избавиться от кучи вызовов разных readmodel и получить большинство (возможно, и всю информацию) за одно обращение к readmodel
        //                               Отдельно нужно рассмотреть подмешивание локализованых данных
        public OrderPositionWithSchemaDto ViewOrderPosition(long orderId, long pricePositionId, long? orderPositionId, bool includeHidden)
        {
            // TODO {all, 20.03.2014}: при рефакторинге ApplicationService (SRP, перевод на OperartionServices и т.п.) нужно избавиться от кучи вызовов разных readmodel и получить большинство (возможно, и всю информацию) за одно обращение к readmodel
            var positionInfo = _orderReadModel.GetOrderPositionDetailedInfo(orderPositionId, orderId, pricePositionId, includeHidden);

            var rateType = _priceReadModel.GetPricePositionRateType(pricePositionId);

            var firmId = _firmReadModel.GetOrderFirmId(orderId);
            var categoryRate = _calculateCategoryRateOperationService.CalculateCategoryRate(firmId, pricePositionId, false);

            var priceCalulations = _orderReadModel.CalculatePricePerUnit(orderId, categoryRate, positionInfo.PricePositionCost);

            // TODO {all, 25.03.2014}: при рефаторинге данного usecase и переносе фильтрации по рубрикам в readmodel попробовать избавиться от firmReadModel.GetOrgUnitId 
            var destOrgUnitId = _firmReadModel.GetOrgUnitId(firmId);

            // TODO {all, 20.03.2014}: если новая модель продаж приживется в более менее стабильном виде и начнет масштабироваться, при рефакторинге ApplicationService (SRP, перевод на OperartionServices и т.п.) - стоит фильтрацию рубрик вынести в OrderReadModel
            positionInfo.LinkingObjectsSchema.FirmCategories = FilterCategories(positionInfo.LinkingObjectsSchema.FirmCategories, destOrgUnitId, rateType);
            positionInfo.LinkingObjectsSchema.AdditionalCategories = FilterCategories(positionInfo.LinkingObjectsSchema.AdditionalCategories,
                                                                                      destOrgUnitId,
                                                                                      rateType);

            return new OrderPositionWithSchemaDto
                {
                    OrderReleaseCountFact = positionInfo.ReleaseCountFact,
                    OrderReleaseCountPlan = positionInfo.ReleaseCountPlan,
                    PlatformName = positionInfo.Platform,
                    PricePositionAmount = positionInfo.Amount,
                    AmountSpecificationMode = positionInfo.AmountSpecificationMode,
                    PricePositionCost = positionInfo.PricePositionCost,
                    IsPositionComposite = positionInfo.IsComposite,
                    IsBudget = positionInfo.IsBudget,
                    LinkingObjectsSchema = positionInfo.LinkingObjectsSchema,
                    PricePerUnit = priceCalulations.PricePerUnit,
                    VatRatio = priceCalulations.VatRatio,
                    RateType = rateType
                };
        }

        private IEnumerable<LinkingObjectsSchemaDto.CategoryDto> FilterCategories(IEnumerable<LinkingObjectsSchemaDto.CategoryDto> categories,
                                                                                  long destOrgUnitId,
                                                                                  PricePositionRateType rateType)
        {
            return categories.Where(x => _supportedCategoriesChecker.IsSupported(rateType, x.Id, destOrgUnitId));
        }
    }
}