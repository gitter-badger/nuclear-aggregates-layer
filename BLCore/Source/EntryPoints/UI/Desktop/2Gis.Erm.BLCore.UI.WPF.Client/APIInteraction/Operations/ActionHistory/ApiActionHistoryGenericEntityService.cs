﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Newtonsoft.Json;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.ActionHistory
{
    public sealed class RestApiActionsHistoryService : RestApiOperationServiceBase, IActionsHistoryService
    {
        public RestApiActionsHistoryService(IApiClient apiClient, ICommonLog logger)
            : base(apiClient, logger, "ActionsHistory.svc")
        {
        }

        public ActionsHistoryDto GetActionHistory(IEntityType entityName, long entityId)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}", entityName, entityId);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Get(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", entityName, entityId), Logger);
            return !string.IsNullOrEmpty(response.ResultContent) ? JsonConvert.DeserializeObject<ActionsHistoryDto>(response.ResultContent, new DotNetDateTimeConverter()) : null;
        }
    }
}
