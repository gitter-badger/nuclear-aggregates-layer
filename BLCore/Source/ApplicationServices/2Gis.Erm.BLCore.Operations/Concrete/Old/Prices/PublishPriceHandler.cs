using System;

using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Prices
{
    public sealed class PublishPriceHandler : RequestHandler<PublishPriceRequest, EmptyResponse>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;
        private readonly IPriceRepository _priceRepository;

        public PublishPriceHandler(IOperationScopeFactory operationScopeFactory,
                                   IPriceReadModel priceReadModel,
                                   IOrganizationUnitReadModel organizationUnitReadModel,
                                   IPriceRepository priceRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
            _priceRepository = priceRepository;
        }

        protected override EmptyResponse Handle(PublishPriceRequest request)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<PublishPriceIdentity>())
            {
                ValidatePrice(request.PriceId, request.OrganizarionUnitId, request.BeginDate, request.PublishDate);

                var price = _priceReadModel.GetPrice(request.PriceId);
                if (price == null)
                {
                    throw new EntityNotFoundException(typeof(Price), request.PriceId);    
                }

                _priceRepository.Publish(price, request.OrganizarionUnitId, request.BeginDate, request.PublishDate);

                operationScope.Complete();

            return Response.Empty;
        }
    }

        // TODO {all, 12.03.2014}: ¬алидаци€ прайса выполн€етс€ и при публикации и при сохранении. Ќужно вынести эту логику отдельно
        // COMMENT {all, 13.08.2014}: Ќужно разделить логику сохранени€ и публикации - сейчас публикаци€ включает в себ€ сохранение полей organizationUnitId, beginDate, publishDate - причЄм без контрол€ timestamp
        private void ValidatePrice(long priceId, long organizationUnitId, DateTime beginDate, DateTime publishDate)
        {
            if (priceId == 0)
            {
                throw new NotificationException(BLResources.PriceIsNeedToBeSavedBeforePublishing);
            }

            if (publishDate < DateTime.UtcNow.Date)
            {
                throw new BusinessLogicException(BLResources.CantPublishOverduePrice);
            }

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

            var priceValidationDto = _priceReadModel.GetPriceValidationDto(priceId);
            if (priceValidationDto.IsPriceDeleted)
            {
                throw new BusinessLogicException(BLResources.CantPublishInactivePrice);
            }

            if (priceValidationDto.IsDeniedPositionsNotValid)
            {
                throw new BusinessLogicException(BLResources.DeniedPositionsReferencesToInactivePositions);
            }

            if (priceValidationDto.IsPricePositionsNotValid)
            {
                throw new BusinessLogicException(BLResources.PricePositionsReferencesToInactivePositions);
            }

            if (priceValidationDto.IsAssociatedPositionsNotValid)
            {
                throw new BusinessLogicException(BLResources.AssociatedPositionsReferencesToInactivePositions);
            }
        }
    }
}