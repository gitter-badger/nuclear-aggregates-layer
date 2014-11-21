using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Territories
{
    public sealed class ValidateTerritoryAvailabilityHandler : RequestHandler<ValidateTerritoryAvailabilityRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;

        public ValidateTerritoryAvailabilityHandler(ISubRequestProcessor subRequestProcessor)
        {
            _subRequestProcessor = subRequestProcessor;
        }

        protected override EmptyResponse Handle(ValidateTerritoryAvailabilityRequest request)
        {
            var response = (SelectCurrentUserTerritoriesExpressionResponse)_subRequestProcessor.HandleSubRequest(new SelectCurrentUserTerritoriesExpressionRequest(), Context);

            var territoryExists = response.TerritoryIds.Contains(request.TerritoryId);
            if (!territoryExists)
            {
                throw new NotificationException(BLResources.TerritoryChangeAccessRightsNotEnought);
            }

            return Response.Empty;
        }
    }
}