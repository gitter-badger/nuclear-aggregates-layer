using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Newtonsoft.Json;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Assign
{
    public sealed class RestApiAssignGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IAssignGenericEntityService<TEntity>
        where TEntity : class, IEntityKey, ICuratedEntity
    {
        public RestApiAssignGenericEntityService(IApiClient apiClient, ITracer logger)
            : base(apiClient, logger, "Assign.svc")
        {
        }

        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}/{2}/{3}/{4}", EntityName, entityId, ownerCode, bypassValidation, isPartialAssign);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Logger);
            return !string.IsNullOrEmpty(response.ResultContent) ? JsonConvert.DeserializeObject<AssignResult>(response.ResultContent) : null;
        }
    }
}
