using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditBargainFileHandler : RequestHandler<EditRequest<BargainFile>, EmptyResponse>
    {
        private readonly IBargainRepository _bargainRepository;

        public EditBargainFileHandler(IBargainRepository bargainRepository)
        {
            _bargainRepository = bargainRepository;
        }

        protected override EmptyResponse Handle(EditRequest<BargainFile> request)
        {
            _bargainRepository.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}