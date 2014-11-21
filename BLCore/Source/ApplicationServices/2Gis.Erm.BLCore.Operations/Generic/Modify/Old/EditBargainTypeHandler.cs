using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Bargains;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditBargainTypeHandler : RequestHandler<EditRequest<BargainType>, EmptyResponse>
    {
        private readonly IBargainTypeService _bargainTypeService;

        public EditBargainTypeHandler(IBargainTypeService bargainTypeService)
        {
            _bargainTypeService = bargainTypeService;
        }

        protected override EmptyResponse Handle(EditRequest<BargainType> request)
        {
            _bargainTypeService.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}