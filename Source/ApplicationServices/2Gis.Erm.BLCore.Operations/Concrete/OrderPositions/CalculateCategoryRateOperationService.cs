using System;

using DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositions
{
    // TODO {a.tukaev, 25.03.2014}: Лучше чтобы в назание абстракций и реализаций для OperationsService оканчивались именно на OperationService, т.к. сервисов разного вида у нас много 
    public class CalculateCategoryRateOperationService : ICalculateCategoryRateOperationService
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public CalculateCategoryRateOperationService(IPriceReadModel priceReadModel, IFirmReadModel firmReadModel)
        {
            _priceReadModel = priceReadModel;
            _firmReadModel = firmReadModel;
        }

        // TODO {a.tukaev, 25.03.2014}: Возможно стоит данный OperationService и оба его метода трансормировать в два отдельных методf pricereadmodel, имеющих специфичную сигнатуру (фактически прототипы для них уже есть GetCategoryRateByFirm и GetCategoryRateByCategory)
        // + отпадет необходимость в strictmode, проверив входные параметры можно будет бросать businesslogicexception
        public decimal CalculateCategoryRate(long firmId, long pricePositionId, long? categoryId, bool strictMode)
        {
            var rateType = _priceReadModel.GetPricePositionRateType(pricePositionId);
            switch (rateType)
            {
                case PricePositionRateType.MostExpensiveCategory:
                {
                    return _priceReadModel.GetCategoryRateByFirm(firmId);
                }
                case PricePositionRateType.BoundCategory:
                {
                    var organizationUnitId = _firmReadModel.GetOrgUnitId(firmId);
                    if (categoryId == null)
                    {
                        if (strictMode)
                        {
                            throw new NotificationException(BLResources.CategoryShouldBeSpecifiedForTheBoundCategoryRateType);
                        }

                        // Если не выбрана ни одна рубрика, берем коэффициент 1: https://confluence.2gis.ru/pages/viewpage.action?pageId=133467722
                        return 1m;
                    }

                    return _priceReadModel.GetCategoryRateByCategory(categoryId.Value, organizationUnitId);
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // TODO {a.tukaev, 25.03.2014}: Возможно стоит данный OperationService и оба его метода трансормировать в два отдельных методf pricereadmodel, имеющих специфичную сигнатуру (фактически прототипы для них уже есть GetCategoryRateByFirm и GetCategoryRateByCategory)
        // + отпадет необходимость в strictmode, проверив входные параметры можно будет бросать businesslogicexception
        public decimal CalculateCategoryRate(long firmId, long pricePositionId, bool strictMode)
        {
            return CalculateCategoryRate(firmId, pricePositionId, null, strictMode);
        }
    }
}
