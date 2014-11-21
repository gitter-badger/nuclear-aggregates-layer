using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Territories
{
    public sealed class SelectCurrentUserTerritoriesHandler : RequestHandler<SelectCurrentUserTerritoriesRequest, SelectCurrentUserTerritoriesResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;

        public SelectCurrentUserTerritoriesHandler(ISubRequestProcessor subRequestProcessor)
        {
            _subRequestProcessor = subRequestProcessor;
        }

        protected override SelectCurrentUserTerritoriesResponse Handle(SelectCurrentUserTerritoriesRequest request)
        {
            var response = (SelectCurrentUserTerritoriesExpressionResponse)_subRequestProcessor.HandleSubRequest(new SelectCurrentUserTerritoriesExpressionRequest(), Context);

            return new SelectCurrentUserTerritoriesResponse { TerritoryIds = response.TerritoryIds };
        }
    }
}