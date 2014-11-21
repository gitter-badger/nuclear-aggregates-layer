namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure
{
    public class ApiResponse<TResultData> : ApiResponse
        where TResultData : new()
    {
        private readonly TResultData _resultData;

        public ApiResponse(bool isSuccessfull, int resultCode, string resultContent, TResultData resultData)
            : base(isSuccessfull, resultCode, resultContent)
        {
            _resultData = resultData;
        }

        public TResultData Data 
        { 
            get
            {
                return _resultData;
            }
        }
    }
}