using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Newtonsoft.Json;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.ChangeClient
{
    public sealed class RestApiChangeClientGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IChangeGenericEntityClientService<TEntity>
        where TEntity : class, IEntityKey
    {
        public RestApiChangeClientGenericEntityService(IApiClient apiClient, ITracer tracer)
            : base(apiClient, tracer, "ChangeClient.svc")
        {
        }

        public ChangeEntityClientValidationResult Validate(long entityId, long clientId)
        {
            var apiTargetResource = GetOperationApiTargetResource("Validate", EntityName.ToString(), entityId, clientId);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            if (!response.IsSuccessfull)
            {
                Tracer.Error(response.ErrorException, "Api operation execution failed. " + apiTargetResource);
                return null;
            }

            return !string.IsNullOrEmpty(response.ResultContent) ? JsonConvert.DeserializeObject<ChangeEntityClientValidationResult>(response.ResultContent) : null;
        }

        public ChangeEntityClientResult Execute(long entityId, long clientId, bool bypassValidation)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}/{2}/{3}", EntityName, entityId, clientId, bypassValidation);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Tracer);
            return !string.IsNullOrEmpty(response.ResultContent) ? JsonConvert.DeserializeObject<ChangeEntityClientResult>(response.ResultContent) : null;
        }
    }
}
