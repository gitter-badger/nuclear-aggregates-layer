using System.Threading.Tasks;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure
{
    public interface IApiClient : IInteractionParty
    {
        ApiResponse Get(ApiRequest request);
        ApiResponse Post(ApiRequest request);
        ApiResponse<TData> Get<TData>(ApiRequest request) where TData : new();
        ApiResponse<TData> Post<TData>(ApiRequest request) where TData : new();

        Task<ApiResponse> GetAsync(ApiRequest request);
        Task<ApiResponse> PostAsync(ApiRequest request);
        Task<ApiResponse<TResultData>> GetAsync<TResultData>(ApiRequest request) where TResultData : new();
        Task<ApiResponse<TResultData>> PostAsync<TResultData>(ApiRequest request) where TResultData : new();
    }

    public sealed class NullApiClient : IApiClient
    {
        public InteractionModel InteractionModel
        {
            get { return InteractionModel.NotSet; }
        }

        public ApiResponse Get(ApiRequest request)
        {
            return new ApiResponse(false, 500, string.Empty);
        }

        public ApiResponse Post(ApiRequest request)
        {
            return new ApiResponse(false, 500, string.Empty);
        }

        public ApiResponse<TData> Get<TData>(ApiRequest request) where TData : new()
        {
            return new ApiResponse<TData>(false, 500, string.Empty, default(TData));
        }

        public ApiResponse<TData> Post<TData>(ApiRequest request) where TData : new()
        {
            return new ApiResponse<TData>(false, 500, string.Empty, default(TData));
        }

        public Task<ApiResponse> GetAsync(ApiRequest request)
        {
            return null;
        }

        public Task<ApiResponse> PostAsync(ApiRequest request)
        {
            return null;
        }

        public Task<ApiResponse<TResultData>> GetAsync<TResultData>(ApiRequest request) where TResultData : new()
        {
            return null;
        }

        public Task<ApiResponse<TResultData>> PostAsync<TResultData>(ApiRequest request) where TResultData : new()
        {
            return null;
        }
    }
}