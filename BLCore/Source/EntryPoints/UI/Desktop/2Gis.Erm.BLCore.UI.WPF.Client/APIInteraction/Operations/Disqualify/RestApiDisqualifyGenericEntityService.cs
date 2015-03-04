using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Newtonsoft.Json;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Disqualify
{
    public sealed class RestApiDisqualifyGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IDisqualifyGenericEntityService<TEntity>
        where TEntity : class, IEntityKey
    {
        public RestApiDisqualifyGenericEntityService(IApiClient apiClient, ITracer tracer)
            : base(apiClient, tracer, "Disqualify.svc")
        {
        }

        public DisqualifyResult Disqualify(long entityId, bool bypassValidation)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}/{2}", EntityName, entityId, bypassValidation);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Tracer);
            return !string.IsNullOrEmpty(response.ResultContent) ? JsonConvert.DeserializeObject<DisqualifyResult>(response.ResultContent) : null;
        }
    }
}
