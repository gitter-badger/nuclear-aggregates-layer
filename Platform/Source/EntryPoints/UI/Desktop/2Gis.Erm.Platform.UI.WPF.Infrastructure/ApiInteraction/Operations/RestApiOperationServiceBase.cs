using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations
{
    public abstract class RestApiOperationServiceBase
    {
        private readonly IApiClient _apiClient;
        private readonly ITracer _tracer;
        private readonly string _operationApiTargetResource;

        protected RestApiOperationServiceBase(IApiClient apiClient, ITracer tracer, string operationApiTargetResource)
        {
            _apiClient = apiClient;
            _tracer = tracer;
            _operationApiTargetResource = operationApiTargetResource;
        }

        protected IApiClient ApiClient
        {
            get
            {
                return _apiClient;
            }
        }

        protected ITracer Tracer
        {
            get
            {
                return _tracer;
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
