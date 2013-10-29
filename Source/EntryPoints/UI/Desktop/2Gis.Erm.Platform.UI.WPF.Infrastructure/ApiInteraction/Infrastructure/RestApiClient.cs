using System.Net;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;

using RestSharp;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure
{
    public sealed class RestApiClient : IApiClient
    {
        private const string RestApiClientIndicator = "RestApiClient";
        private readonly RestClient _restClient;

        public RestApiClient(ICommonSettings settings)
        {
            var cookieContainer = new CookieContainer();
            _restClient = new RestClient 
                {
                    BaseUrl = settings.ApiUrl,
                    Authenticator = new NtlmAuthenticator(), 
                    CookieContainer = cookieContainer, 
                    UserAgent = RestApiClientIndicator + "_RestSharp"
                };
        }

        public InteractionModel InteractionModel
        {
            get { return InteractionModel.Rest; }
        }

        public ApiResponse Get(ApiRequest request)
        {
            return _restClient
                .Execute(request.ToRestRequest(Method.GET))
                .ToApiResponse();
        }

        public ApiResponse Post(ApiRequest request)
        {
            return _restClient
                .Execute(request.ToRestRequest(Method.POST))
                .ToApiResponse();
        }

        public ApiResponse<TResultData> Get<TResultData>(ApiRequest request) 
            where TResultData : new()
        {
            return _restClient
                .Execute<TResultData>(request.ToRestRequest(Method.GET))
                .ToApiResponse<TResultData>();
        }

        public ApiResponse<TResultData> Post<TResultData>(ApiRequest request) where TResultData : new()
        {
            return _restClient
                .Execute<TResultData>(request.ToRestRequest(Method.POST))
                .ToApiResponse<TResultData>();
        }

        public Task<ApiResponse> GetAsync(ApiRequest request)
        {
            var tcs = new TaskCompletionSource<ApiResponse>();

            _restClient.ExecuteAsync(
                request.ToRestRequest(Method.GET),
                response => tcs.TrySetResult(response.ToApiResponse()));

            return tcs.Task;
        }

        public Task<ApiResponse> PostAsync(ApiRequest request)
        {
            var tcs = new TaskCompletionSource<ApiResponse>();

            _restClient.ExecuteAsync(
                request.ToRestRequest(Method.POST),
                response => tcs.TrySetResult(response.ToApiResponse()));

            return tcs.Task;
        }

        public Task<ApiResponse<TResultData>> ExecuteTaskAsync<TResultData>(ApiRequest request)
            where TResultData : new()
        {
            var tcs = new TaskCompletionSource<ApiResponse<TResultData>>();

            _restClient.ExecuteAsync<TResultData>(
                request.ToRestRequest(Method.POST),
                response => tcs.TrySetResult(response.ToApiResponse()));

            return tcs.Task;
        }

        public Task<ApiResponse<TResultData>> GetAsync<TResultData>(ApiRequest request) 
            where TResultData : new()
        {
            var tcs = new TaskCompletionSource<ApiResponse<TResultData>>();

            _restClient.ExecuteAsync<TResultData>(
                request.ToRestRequest(Method.GET),
                response => tcs.TrySetResult(response.ToApiResponse()));

            return tcs.Task;
        }

        public Task<ApiResponse<TResultData>> PostAsync<TResultData>(ApiRequest request) 
            where TResultData : new()
        {
            var tcs = new TaskCompletionSource<ApiResponse<TResultData>>();

            _restClient.ExecuteAsync<TResultData>(
                request.ToRestRequest(Method.POST),
                response => tcs.TrySetResult(response.ToApiResponse()));

            return tcs.Task;
        }
    }
}
