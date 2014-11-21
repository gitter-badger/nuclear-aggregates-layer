using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public interface IUseCase : IDisposable
    {
        Guid Id { get; }
        object FactoriesContext { get; }

        IUseCaseState State { get; }

        MessageProcessingResult<TResult> Send<TResult>(IMessage message);
        bool Post(IMessage message);
    }
}
