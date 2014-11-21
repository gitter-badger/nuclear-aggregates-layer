﻿using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations
{
    public abstract class RestApiOperationServiceBase
    {
        private readonly IApiClient _apiClient;
        private readonly ICommonLog _logger;
        private readonly string _operationApiTargetResource;

        protected RestApiOperationServiceBase(IApiClient apiClient, ICommonLog logger, string operationApiTargetResource)
        {
            _apiClient = apiClient;
            _logger = logger;
            _operationApiTargetResource = operationApiTargetResource;
        }

        protected IApiClient ApiClient
        {
            get
            {
                return _apiClient;
            }
        }

        protected ICommonLog Logger
        {
            get
            {
                return _logger;
            }
        }

        protected string GetOperationApiTargetResource(string resourceTemplate, params object[] resourceParameters)
        {
            if (resourceParameters == null || resourceParameters.Length == 0)
            {
                return _operationApiTargetResource;
            }

            return string.Format("{0}/{1}/{2}", _operationApiTargetResource, _apiClient.InteractionModel, string.Format(resourceTemplate, resourceParameters));
        }
    }
}
