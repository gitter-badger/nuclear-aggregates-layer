using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public interface IUseCaseManager
    {
        MessageProcessingResult<TResult> Send<TResult>(Guid useCaseToken, IMessage message);
        bool Post(Guid useCaseToken, IMessage message);
    }
}
