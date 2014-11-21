using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditDeniedPositionHandler : RequestHandler<EditRequest<DeniedPosition>, EmptyResponse>
    {
        private readonly IPriceRepository _priceRepository;

        public EditDeniedPositionHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        protected override EmptyResponse Handle(EditRequest<DeniedPosition> request)
        {
            var deniedPosition = request.Entity;
            _priceRepository.CreateOrUpdate(deniedPosition);

            return Response.Empty;
        }
    }
}