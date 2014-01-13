using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditPricePositionHandler : RequestHandler<EditRequest<PricePosition>, EmptyResponse>
    {
        private readonly IPriceRepository _priceRepository;
        public EditPricePositionHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        protected override EmptyResponse Handle(EditRequest<PricePosition> request)
        {
            var pricePosition = request.Entity;
            if (pricePosition.MinAdvertisementAmount.HasValue && pricePosition.MinAdvertisementAmount < 0)
            {
                throw new NotificationException(BLResources.MinAdvertisementAmountCantbeLessThanZero);
            }
                
            if (pricePosition.MaxAdvertisementAmount.HasValue && pricePosition.MinAdvertisementAmount.HasValue &&
                pricePosition.MaxAdvertisementAmount < pricePosition.MinAdvertisementAmount)
            {
                throw new NotificationException(BLResources.MaxAdvertisementAmountCantBeLessThanMinAdvertisementAmount);
            }

            _priceRepository.CreateOrUpdate(pricePosition);
            return Response.Empty;
        }
    }
}