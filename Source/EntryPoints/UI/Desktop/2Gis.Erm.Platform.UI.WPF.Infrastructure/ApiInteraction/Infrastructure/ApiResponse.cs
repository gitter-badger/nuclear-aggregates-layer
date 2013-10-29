using System;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure
{
    public class ApiResponse
    {
        private readonly bool _isSuccessfull;
        private readonly int _resultCode;
        private readonly string _resultContent;

        public ApiResponse(bool isSuccessfull, int resultCode, string resultContent)
        {
            _resultCode = resultCode;
            _resultContent = resultContent;
            _isSuccessfull = isSuccessfull;
        }

        public bool IsSuccessfull
        {
            get
            {
                return _isSuccessfull;
            }
        }

        public int ResultCode
        {
            get
            {
                return _resultCode;
            }
        }

        public string ResultContent
        {
            get
            {
                return _resultContent;
            }
        }
        
        public Exception ErrorException { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusDescription { get; set; }
    }
}