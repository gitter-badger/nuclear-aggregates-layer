using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditPositionCategoryHandler : RequestHandler<EditRequest<PositionCategory>, EmptyResponse>
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public EditPositionCategoryHandler(IPositionRepository positionRepository, IOperationScopeFactory scopeFactory)
        {
            _positionRepository = positionRepository;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(EditRequest<PositionCategory> request)
        {
            var positionCategory = request.Entity;

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(positionCategory))
            {
                _positionRepository.CreateOrUpdate(positionCategory);
                scope.Complete();
            }

            return Response.Empty;
        }
    }
}