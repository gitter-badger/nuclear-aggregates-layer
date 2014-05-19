using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;

using System;

using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditPriceHandler : RequestHandler<EditRequest<Price>, EmptyResponse>
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;
        private readonly ICreatePriceAggregateService _createPriceAggregateService;
        private readonly IUpdateAggregateRepository<Price> _updatePriceAggregateRepository;

        public EditPriceHandler(IPriceReadModel priceReadModel,
                                IOrganizationUnitReadModel organizationUnitReadModel,
                                ICreatePriceAggregateService createPriceAggregateService,
                                IUpdateAggregateRepository<Price> updatePriceAggregateRepository)
        {
            _priceReadModel = priceReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
            _createPriceAggregateService = createPriceAggregateService;
            _updatePriceAggregateRepository = updatePriceAggregateRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Price> request)
        {
            var price = request.Entity;
            ValidatePrice(price.Id, price.OrganizationUnitId, price.BeginDate, price.PublishDate);

            if (price.IsNew())
            {
                var currencyId = _organizationUnitReadModel.GetCurrencyId(price.OrganizationUnitId);
                _createPriceAggregateService.Create(price, currencyId);
            }
            else
            {
                _updatePriceAggregateRepository.Update(price);
            }

            return Response.Empty;
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

            var isPriceExist = _priceReadModel.IsDifferentPriceExistsForDate(priceId, organizationUnitId, beginDate);
            if (isPriceExist)
            {
                var organizationUnitName = _organizationUnitReadModel.GetName(organizationUnitId);
                throw new NotificationException(string.Format(BLResources.PriceForOrgUnitExistsForDate, organizationUnitName, beginDate.ToShortDateString()));
            }
        }
    }
}