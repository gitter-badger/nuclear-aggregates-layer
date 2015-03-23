using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.ChangeTerritory
{
    public sealed class RestApiChangeTerritoryGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IChangeGenericEntityTerritoryService<TEntity>
        where TEntity : class, IEntityKey
    {
        public RestApiChangeTerritoryGenericEntityService(IApiClient apiClient, ITracer tracer)
            : base(apiClient, tracer, "ChangeTerritory.svc")
        {
        }

        public void ChangeTerritory(long entityId, long territoryId)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}/{2}", EntityName, entityId, territoryId);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Tracer);
        }
    }
}
