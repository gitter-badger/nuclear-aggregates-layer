using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAssociatedPositionsGroupHandler : RequestHandler<EditRequest<AssociatedPositionsGroup>, EmptyResponse>
    {
        private readonly IPriceRepository _priceRepository;

        public EditAssociatedPositionsGroupHandler(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        protected override EmptyResponse Handle(EditRequest<AssociatedPositionsGroup> request)
        {
            _priceRepository.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}