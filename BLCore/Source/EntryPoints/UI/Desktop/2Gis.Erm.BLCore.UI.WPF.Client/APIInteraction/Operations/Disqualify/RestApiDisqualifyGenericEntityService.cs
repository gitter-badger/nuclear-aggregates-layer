using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.Platform.Common.Logging;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Disqualify
{
    public sealed class RestApiDisqualifyGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IDisqualifyGenericEntityService<TEntity>
        where TEntity : class, IEntityKey
    {
        public RestApiDisqualifyGenericEntityService(IApiClient apiClient, ICommonLog logger)
            : base(apiClient, logger, "Disqualify.svc")
        {
        }

        public DisqualifyResult Disqualify(long entityId, bool bypassValidation)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}/{2}", EntityName, entityId, bypassValidation);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Logger);
            return !string.IsNullOrEmpty(response.ResultContent) ? JsonConvert.DeserializeObject<DisqualifyResult>(response.ResultContent) : null;
        }
    }
}
