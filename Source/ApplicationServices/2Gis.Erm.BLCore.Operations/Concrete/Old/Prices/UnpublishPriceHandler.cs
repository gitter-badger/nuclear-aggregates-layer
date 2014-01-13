using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.UseCases;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Prices
{
    [UseCase(Duration = UseCaseDuration.Long)]
    public sealed class UnpublishPriceHandler : RequestHandler<UnpublishPriceRequest, EmptyResponse>
    {
        private readonly IPriceRepository _priceRepository;

        public UnpublishPriceHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        protected override EmptyResponse Handle(UnpublishPriceRequest request)
        {
            _priceRepository.Unpublish(request.PriceId);
            return Response.Empty;
        }
    }
}