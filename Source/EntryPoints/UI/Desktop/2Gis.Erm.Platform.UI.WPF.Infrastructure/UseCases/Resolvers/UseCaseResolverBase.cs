using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Resolvers
{
    public abstract class UseCaseResolverBase<TMessage> : IUseCaseResolver<TMessage> 
        where TMessage : class, IMessage
    {
        public bool IsAppropriate(IUseCase checkingUseCase, IMessage message)
        {
            var concreteMessage = message as TMessage;
            if (concreteMessage == null)
            {
                return false;
            }

            return IsAppropriate(checkingUseCase, concreteMessage);
        }

        protected abstract bool IsAppropriate(IUseCase checkingUseCase, TMessage message);
    }
}