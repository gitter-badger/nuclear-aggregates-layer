using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditPositionChildrenHandler : RequestHandler<EditRequest<PositionChildren>, EmptyResponse>
    {
        private readonly IPositionRepository _positionRepository;

        public EditPositionChildrenHandler(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        protected override EmptyResponse Handle(EditRequest<PositionChildren> request)
        {
            var positionChildren = request.Entity;
            _positionRepository.CreateOrUpdate(positionChildren);

            return Response.Empty;
        }
    }
}