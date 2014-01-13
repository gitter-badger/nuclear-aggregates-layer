using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAssociatedPositionHandler : RequestHandler<EditRequest<AssociatedPosition>, EmptyResponse>
    {
        private readonly IPriceRepository _priceRepository;

        public EditAssociatedPositionHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        protected override EmptyResponse Handle(EditRequest<AssociatedPosition> request)
        {
            var sssociatedPosition = request.Entity;
            _priceRepository.CreateOrUpdate(sssociatedPosition);

            return Response.Empty;
        }
    }
}