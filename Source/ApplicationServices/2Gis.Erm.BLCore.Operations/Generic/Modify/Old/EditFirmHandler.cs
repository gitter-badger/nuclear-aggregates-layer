using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditFirmHandler : RequestHandler<EditRequest<Firm>, EmptyResponse>
    {
        private readonly IFirmRepository _firmRepository;

        public EditFirmHandler(IFirmRepository firmRepository)
        {
            _firmRepository = firmRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Firm> request)
        {
            _firmRepository.Update(request.Entity);
            return Response.Empty;
        }
    }
}