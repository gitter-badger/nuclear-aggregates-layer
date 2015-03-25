using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Newtonsoft.Json;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Qualify
{
    public sealed class RestApiQualifyGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IQualifyGenericEntityService<TEntity>
        where TEntity : class, IEntityKey
    {
        public RestApiQualifyGenericEntityService(IApiClient apiClient, ITracer tracer)
            : base(apiClient, tracer, "Qualify.svc")
        {
        }

        public QualifyResult Qualify(long entityId, long ownerCode, long? relatedEntityId)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}/{2}/{3}", EntityName, entityId, ownerCode, relatedEntityId);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Tracer);
            int? resultRelatedEntityId = !string.IsNullOrEmpty(response.ResultContent) ? JsonConvert.DeserializeObject<int?>(response.ResultContent) : null;
            return new QualifyResult { RelatedEntityId = resultRelatedEntityId };
        }
    }
}
