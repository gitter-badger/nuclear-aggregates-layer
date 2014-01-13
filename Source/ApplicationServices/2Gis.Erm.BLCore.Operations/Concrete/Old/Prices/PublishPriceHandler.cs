using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Prices
{
    public sealed class PublishPriceHandler : RequestHandler<PublishPriceRequest, EmptyResponse>
    {
        private readonly IPriceRepository _priceRepository;

        public PublishPriceHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        protected override EmptyResponse Handle(PublishPriceRequest request)
        {
            _priceRepository.Publish(request.PriceId, request.OrganizarionUnitId, request.BeginDate, request.PublishDate);
            return Response.Empty;
        }
    }
}