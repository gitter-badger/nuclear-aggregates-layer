using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Activate
{
    public sealed class RestApiActivateGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IActivateGenericEntityService<TEntity>
        where TEntity : class, IEntityKey, IDeactivatableEntity
    {
        public RestApiActivateGenericEntityService(IApiClient apiClient, ITracer tracer)
            : base(apiClient, tracer, "Activate.svc")
        {
        }

        public int Activate(long entityId)
        {
            const int StubReturnValue = 0;
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}", EntityName, entityId);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Tracer);
            return StubReturnValue;
        }
    }
}
