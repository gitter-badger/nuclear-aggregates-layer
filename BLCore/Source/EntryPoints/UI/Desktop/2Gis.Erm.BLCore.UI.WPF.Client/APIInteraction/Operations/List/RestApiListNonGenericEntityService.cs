using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.List
{
    public class RestApiListNonGenericEntityService : RestApiOperationServiceBase, IListNonGenericEntityService
    {
        public RestApiListNonGenericEntityService(IApiClient apiClient, ITracer logger)
            : base(apiClient, logger, "List.svc")
        {
        }

        #region Implementation of IListNonGenericEntityService

        public ListResult List(EntityName entityName, SearchListModel searchListModel)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}", entityName);
            var request = new ApiRequest(apiTargetResource);
            request.AddParametersFromInstance(searchListModel);
            var response = ApiClient.Get(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource, Logger);
            return ListResultJsonParsers.ParseListResult(response.ResultContent);
        }

        #endregion
    }
}