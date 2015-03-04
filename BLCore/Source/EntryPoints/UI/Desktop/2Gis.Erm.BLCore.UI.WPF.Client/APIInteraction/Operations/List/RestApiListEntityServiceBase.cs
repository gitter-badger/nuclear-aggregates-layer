using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.List
{
    public abstract class RestApiListEntityServiceBase<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IListEntityService
        where TEntity : class, IEntityKey
    {
        protected RestApiListEntityServiceBase(IApiClient apiClient, ICommonLog logger)
            : base(apiClient, logger, "List.svc")
        {
        }

        public abstract IRemoteCollection List(SearchListModel searchListModel);

        protected ListResult GetList(SearchListModel searchListModel)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}", EntityName);
            var request = new ApiRequest(apiTargetResource);
            request.AddParametersFromInstance(searchListModel);
            var response = ApiClient.Get(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource, Logger);
            return ListResultJsonParsers.ParseListResult(response.ResultContent);
        }
    }
}