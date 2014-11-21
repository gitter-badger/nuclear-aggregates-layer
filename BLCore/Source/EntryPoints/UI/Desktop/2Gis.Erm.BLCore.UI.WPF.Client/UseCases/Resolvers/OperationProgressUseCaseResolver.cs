using System.Linq;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Resolvers;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Resolvers
{
    public sealed class OperationProgressUseCaseResolver : UseCaseResolverBase<OperationProgressMessage>
    {
        protected override bool IsAppropriate(IUseCase checkingUseCase, OperationProgressMessage message)
        {
            var processings = checkingUseCase.State.ProcessingsRegistry.Processings;
            if (processings == null)
            {
                return false;
            }

            return processings.Any(p => message.OperationToken == p.Id);
        }
    }
}