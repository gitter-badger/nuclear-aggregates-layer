using System;

using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Price;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public sealed class ModifyPriceOperationService : IModifyBusinessModelEntityService<Price>
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;
        private readonly ICreatePriceAggregateService _createPriceAggregateService;
        private readonly IUpdatePriceAggregateService _updatePriceAggregateService;
        private readonly IBusinessModelEntityObtainer<Price> _priceObtainer;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ModifyPriceOperationService(IPriceReadModel priceReadModel,
                                           IOrganizationUnitReadModel organizationUnitReadModel,
                                           ICreatePriceAggregateService createPriceAggregateService,
                                           IUpdatePriceAggregateService updatePriceAggregateService,
                                           IBusinessModelEntityObtainer<Price> priceObtainer,
                                           IOperationScopeFactory operationScopeFactory)
        {
            _priceReadModel = priceReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
            _createPriceAggregateService = createPriceAggregateService;
            _updatePriceAggregateService = updatePriceAggregateService;
            _priceObtainer = priceObtainer;
            _operationScopeFactory = operationScopeFactory;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var price = _priceObtainer.ObtainBusinessModelEntity(domainEntityDto);

            if (price.IsNew())
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Price>())
                {
                    ValidatePrice(price.Id, price.OrganizationUnitId, price.BeginDate, price.PublishDate);
                    
                    var currencyId = _organizationUnitReadModel.GetCurrencyId(price.OrganizationUnitId);
                    _createPriceAggregateService.Create(price, currencyId);
                    
                    scope.Complete();
                }
            }
            else
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Price>())
                {
                    if (price.IsPublished)
                    {
                        throw new PublishedPriceModificationException(BLResources.PublishedPriceCannotBeModified);
                    }

                    ValidatePrice(price.Id, price.OrganizationUnitId, price.BeginDate, price.PublishDate);
                    
                    _updatePriceAggregateService.Update(price);
                    
                    scope.Complete();
                }
            }

            return price.Id;
        }

        // TODO {all, 12.03.2014}: Валидация прайса выполняется и при публикации и при сохранении. Нужно вынести эту логику отдельно
        private void ValidatePrice(long priceId, long organizationUnitId, DateTime beginDate, DateTime publishDate)
        {
            var minimalDate = DateTime.UtcNow.AddMonths(1);

            var lowBoundSatisfied = beginDate.Year > minimalDate.Year || (beginDate.Year == minimalDate.Year && beginDate.Month >= minimalDate.Month);
            if (!lowBoundSatisfied)
            {
                throw new NotificationException(BLResources.BeginMonthMustBeGreaterOrEqualThanNextMonth);
            }

            if (beginDate < publishDate)
            {
                throw new NotificationException(string.Format(BLResources.BeginDateMustBeNotLessThan, publishDate.AddDays(1 - publishDate.Day).AddMonths(1).ToShortDateString()));
            }

            if (_priceReadModel.IsDifferentPriceExistsForDate(priceId, organizationUnitId, beginDate))
            {
                var organizationUnitName = _organizationUnitReadModel.GetName(organizationUnitId);
                throw new EntityIsNotUniqueException(typeof(Price), string.Format(BLResources.PriceForOrgUnitExistsForDate, organizationUnitName, beginDate.ToShortDateString()));
            }
        }
    }
}