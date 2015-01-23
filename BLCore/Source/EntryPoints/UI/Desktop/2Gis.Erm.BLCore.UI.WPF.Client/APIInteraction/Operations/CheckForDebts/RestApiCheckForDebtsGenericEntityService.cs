﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.Platform.Common.Logging;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.CheckForDebts
{
    public sealed class RestApiCheckForDebtsGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, ICheckGenericEntityForDebtsService<TEntity>
        where TEntity : class, IEntityKey
    {
        public RestApiCheckForDebtsGenericEntityService(IApiClient apiClient, ICommonLog logger)
            : base(apiClient, logger, "CheckForDebts.svc")
        {
        }

        public CheckForDebtsResult CheckForDebts(long entityId)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}", EntityName, entityId);
            var request = new ApiRequest(apiTargetResource);
            var entities = new { entityIds = new[] { entityId.ToString() } };
            request.AddParametersFromInstance(entities);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Logger);
            return !string.IsNullOrEmpty(response.ResultContent) ? JsonConvert.DeserializeObject<CheckForDebtsResult>(response.ResultContent) : null;
        }
    }
}
