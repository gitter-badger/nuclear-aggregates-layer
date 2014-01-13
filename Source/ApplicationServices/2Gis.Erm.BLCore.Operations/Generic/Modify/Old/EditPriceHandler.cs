using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditPriceHandler : RequestHandler<EditRequest<Price>, EmptyResponse>
    {
        private readonly IPriceRepository _priceRepository;

        public EditPriceHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Price> request)
        {
            var price = request.Entity;
            _priceRepository.CreateOrUpdate(price);
            return Response.Empty;
        }
    }
}