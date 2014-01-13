using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditBargainHandler : RequestHandler<EditRequest<Bargain>, EmptyResponse>
    {
        private readonly IBargainRepository _bargainRepository;

        public EditBargainHandler(IBargainRepository bargainRepository)
        {
            _bargainRepository = bargainRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Bargain> request)
        {
            _bargainRepository.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}