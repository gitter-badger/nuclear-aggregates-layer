using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations
{
    public interface IOperationConfiguratorViewModelFactory
    {
        IOperationConfiguratorViewModel Create<TOperationIdentity>(IUseCase useCase, IEntityType entityName, long[] operationProcessingEntities)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new();

        IOperationConfiguratorViewModel Create(IUseCase useCase, IOperationIdentity operationIdentity, IEntityType entityName, long[] operationProcessingEntities);
    }
}
