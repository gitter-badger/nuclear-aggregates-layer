using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Resolvers
{
    public interface IUseCaseResolversRegistry
    {
        bool TryGetResolvers(IMessage message, out IUseCaseResolver[] resolvers);
    }
}
