using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.Common.Logging;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Delete
{
    public sealed class RestApiDeleteGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IDeleteGenericEntityService<TEntity>
        where TEntity : class, IEntityKey
    {
        public RestApiDeleteGenericEntityService(IApiClient apiClient, ICommonLog logger)
            : base(apiClient, logger, "Delete.svc")
        {
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}", EntityName, entityId);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Logger);
            return !string.IsNullOrEmpty(response.ResultContent) ? JsonConvert.DeserializeObject<DeleteConfirmation>(response.ResultContent, new DotNetDateTimeConverter()) : null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            throw new InvalidOperationException("Get confirmation is obsolete part of DELETE operation");
        }
    }
}
