﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Deactivate
{
    public sealed class RestApiDeactivateGenericEntityService<TEntity> : RestApiOperationEntitySpecificServiceBase<TEntity>, IDeactivateGenericEntityService<TEntity>
        where TEntity : class, IEntityKey, IDeactivatableEntity
    {
        public RestApiDeactivateGenericEntityService(IApiClient apiClient, ICommonLog logger)
            : base(apiClient, logger, "Deactivate.svc")
        {
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}/{2}", EntityName, entityId, ownerCode);
            var request = new ApiRequest(apiTargetResource);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". EntityName: {0}. Id: {1}", EntityName, entityId), Logger);
            return !string.IsNullOrEmpty(response.ResultContent) ? JsonConvert.DeserializeObject<DeactivateConfirmation>(response.ResultContent) : null;
        }
    }
}
