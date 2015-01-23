using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.Common.Logging;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Activate
{
    public sealed class RestApiActivateGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IActivateGenericEntityService<TEntity>
        where TEntity : class, IEntityKey, IDeactivatableEntity
    {
        public RestApiActivateGenericEntityService(IApiClient apiClient, ICommonLog logger)
            : base(apiClient, logger, "Activate.svc")
        {
        }

        public int Activate(long entityId)
        {
            const int StubReturnValue = 0;
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}", EntityName, entityId);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Logger);
            return StubReturnValue;
        }
    }
}
