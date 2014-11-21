using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public sealed class UseCaseMessageSink : IMessageSink
    {
        private readonly IUseCaseManager _useCaseManager;
        private readonly Guid _useCaseToken;

        public UseCaseMessageSink(IUseCaseManager useCaseManager, Guid useCaseToken)
        {
            _useCaseManager = useCaseManager;
            _useCaseToken = useCaseToken;
        }

        public MessageProcessingResult<TResult> Send<TResult>(IMessage message)
        {
            return _useCaseManager.Send<TResult>(_useCaseToken, message);
        }

        public bool Post(IMessage message)
        {
            return _useCaseManager.Post(_useCaseToken, message);
        }
    }
}
