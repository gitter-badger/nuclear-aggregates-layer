using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditPositionCategoryHandler : RequestHandler<EditRequest<PositionCategory>, EmptyResponse>
    {
        private readonly IPositionRepository _positionRepository;

        public EditPositionCategoryHandler(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        protected override EmptyResponse Handle(EditRequest<PositionCategory> request)
        {
            _positionRepository.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}