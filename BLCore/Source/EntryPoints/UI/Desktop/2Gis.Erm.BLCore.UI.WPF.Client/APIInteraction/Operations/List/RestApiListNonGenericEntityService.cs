using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using NuClear.Model.Common.Entities;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.List
{
    public class RestApiListNonGenericEntityService : RestApiOperationServiceBase, IListNonGenericEntityService
    {
        public RestApiListNonGenericEntityService(IApiClient apiClient, ITracer tracer)
            : base(apiClient, tracer, "List.svc")
        {
        }

        #region Implementation of IListNonGenericEntityService

        public ListResult List(IEntityType entityName, SearchListModel searchListModel)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}", entityName);
            var request = new ApiRequest(apiTargetResource);
            request.AddParametersFromInstance(searchListModel);
            var response = ApiClient.Get(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource, Tracer);
            return ListResultJsonParsers.ParseListResult(response.ResultContent);
        }

        #endregion
    }
}