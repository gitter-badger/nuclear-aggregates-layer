using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditPricePositionHandler : RequestHandler<EditRequest<PricePosition>, EmptyResponse>
    {
        private readonly IPositionReadModel _positionReadModel;
        private readonly IPriceReadModel _priceReadModel;
        private readonly ICreatePricePositionAggregateService _createPricePositionAggregateService;
        private readonly IUpdateAggregateRepository<PricePosition> _updatePricePositionAggregateService;

        public EditPricePositionHandler(IPositionReadModel positionReadModel,
                                        IPriceReadModel priceReadModel,
                                        ICreatePricePositionAggregateService createPricePositionAggregateService,
                                        IUpdateAggregateRepository<PricePosition> updatePricePositionAggregateService)
        {
            _positionReadModel = positionReadModel;
            _priceReadModel = priceReadModel;
            _createPricePositionAggregateService = createPricePositionAggregateService;
            _updatePricePositionAggregateService = updatePricePositionAggregateService;
        }

        protected override EmptyResponse Handle(EditRequest<PricePosition> request)
        {
            var pricePosition = request.Entity;

            var priceId = pricePosition.PriceId;
            var positionId = pricePosition.PositionId;

            ValidatePricePosition(pricePosition.Id,
                                  priceId,
                                  positionId,
                                  pricePosition.MinAdvertisementAmount,
                                  pricePosition.MaxAdvertisementAmount,
                                  pricePosition.Amount,
                                  pricePosition.AmountSpecificationMode,
                                  pricePosition.RateType);

            if (pricePosition.IsNew())
            {
                _createPricePositionAggregateService.Create(pricePosition);
            }
            else
            {
                _updatePricePositionAggregateService.Update(pricePosition);
            }

            return Response.Empty;
        }

        private void ValidatePricePosition(long pricePositionId,
                                           long priceId,
                                           long positionId,
                                           int? minAdvertisementAmount,
                                           int? maxAdvertisementAmount,
                                           int? amount,
                                           PricePositionAmountSpecificationMode pricePositionAmountSpecificationMode,
                                           PricePositionRateType pricePositionRateType)
        {
            if (minAdvertisementAmount.HasValue && minAdvertisementAmount < 0)
            {
                throw new NotificationException(BLResources.MinAdvertisementAmountCantbeLessThanZero);
            }
                
            if (maxAdvertisementAmount.HasValue && minAdvertisementAmount.HasValue && maxAdvertisementAmount < minAdvertisementAmount)
            {
                throw new NotificationException(BLResources.MaxAdvertisementAmountCantBeLessThanMinAdvertisementAmount);
            }

            if (pricePositionRateType == PricePositionRateType.BoundCategory)
            {
                var allowedBindingTypesForBoundCategoryRateTypes = new[]
                    {
                        PositionBindingObjectType.CategorySingle,
                        PositionBindingObjectType.AddressCategorySingle,
                        PositionBindingObjectType.AddressFirstLevelCategorySingle,

                        PositionBindingObjectType.CategoryMultipleAsterix,
                        PositionBindingObjectType.AddressCategoryMultiple,
                        PositionBindingObjectType.CategoryMultiple,
                        PositionBindingObjectType.AddressFirstLevelCategoryMultiple,
                    };

                var positionBindingObjectType = _positionReadModel.GetPositionBindingObjectTypes(new[] { positionId }).Single().Value;
                if (!allowedBindingTypesForBoundCategoryRateTypes.Contains(positionBindingObjectType))
                {
                    throw new NotificationException(string.Format(BLResources.CannotUseRateTypeForBindingObjectType,
                                                                  pricePositionRateType.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                                                  positionBindingObjectType.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)));
                }
            }

            if (amount == null && pricePositionAmountSpecificationMode == PricePositionAmountSpecificationMode.FixedValue)
            {
                throw new NotificationException(BLResources.CountMustBeSpecified);
            }

            var isAlreadyCreated = _priceReadModel.DoesPricePositionExist(priceId, positionId, pricePositionId);
            if (isAlreadyCreated)
            {
                throw new NotificationException(BLResources.PricePositionForPositionAlreadyCreated);
            }

            var isAlreadyCreatedWithinNonDeleted = _priceReadModel.DoesPricePositionExistWithinNonDeleted(priceId, positionId, pricePositionId);
            if (isAlreadyCreatedWithinNonDeleted)
            {
                throw new NotificationException(BLResources.HiddenPricePositionForPositionAlreadyCreated);
            }

            var isSupportedByExport = _positionReadModel.IsSupportedByExport(positionId);
            if (!isSupportedByExport)
            {
                throw new NotificationException(BLResources.PricePositionPlatformIsNotSupportedByExport);
            }
        }
    }
}