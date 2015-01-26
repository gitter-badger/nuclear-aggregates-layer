﻿using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Append
{
    public sealed class RestApiAppendGenericEntityService<TAppended, TParent> : RestApiOperationServiceBase, IAppendGenericEntityService<TAppended, TParent>
        where TAppended : class, IEntityKey 
        where TParent : class, IEntityKey
    {
        private readonly IEntityType _appendedEntityName;
        private readonly IEntityType _parentEntityName;

        public RestApiAppendGenericEntityService(IApiClient apiClient, ICommonLog logger)
            : base(apiClient, logger, "Append.svc/Rest/{0}/{1}/{2}/{3}")
        {
            _appendedEntityName = typeof(TAppended).AsEntityName();
            _parentEntityName = typeof(TParent).AsEntityName();
        }

        public void Append(AppendParams appendParams)
        {
            if (!appendParams.AppendedType.Equals(_appendedEntityName))
            {
                throw new InvalidOperationException(string.Format("Invalid type specified as appended. Specified:{0}.Reqiured:{1}.",
                                                                  appendParams.AppendedType,
                                                                  _appendedEntityName));
            }

            if (!appendParams.ParentType.Equals(_parentEntityName))
            {
                throw new InvalidOperationException(string.Format("Invalid type specified as parent. Specified:{0}.Reqiured:{1}.",
                                                                  appendParams.ParentType,
                                                                  _parentEntityName));
            }

            var apiTargetResource = GetOperationApiTargetResource("{0}/{1}/{2}/{3}",
                                                                  appendParams.ParentType,
                                                                  appendParams.ParentId,
                                                                  appendParams.AppendedType,
                                                                  appendParams.AppendedId);
            var request = new ApiRequest(apiTargetResource);
            request.AddParametersFromInstance(appendParams);
            var response = ApiClient.Post(request);
            response.IfErrorThanReportAndThrowException(apiTargetResource + string.Format(". AppendedType: {0}. ParentType: {1}",
                                                                                          _appendedEntityName,
                                                                                          _parentEntityName),
                                                        Logger);
        }
    }
}
