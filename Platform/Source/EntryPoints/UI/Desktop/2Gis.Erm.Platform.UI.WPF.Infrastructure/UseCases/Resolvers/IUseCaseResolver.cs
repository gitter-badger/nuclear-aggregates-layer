using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Resolvers
{
    public interface IUseCaseResolver
    {
        bool IsAppropriate(IUseCase checkingUseCase, IMessage message);
    }

    public interface IUseCaseResolver<TMessage> : IUseCaseResolver
        where TMessage : class, IMessage
    {
    }
}
