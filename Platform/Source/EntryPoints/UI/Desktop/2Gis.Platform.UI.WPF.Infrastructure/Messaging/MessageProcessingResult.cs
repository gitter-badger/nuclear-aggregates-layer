namespace DoubleGis.Platform.UI.WPF.Infrastructure.Messaging
{
    public sealed class MessageProcessingResult<TResult> : IMessageProcessingResult
    {
        private readonly TResult _result;

        public MessageProcessingResult(TResult result)
        {
            _result = result;
        }

        object IMessageProcessingResult.Result
        {
            get
            {
                return _result;
            }
        }

        public TResult Result
        {
            get
            {
                return _result;
            }
        }
    }
}
