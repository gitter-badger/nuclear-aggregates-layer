using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations
{
    public interface IOperationConfiguratorViewModelFactory
    {
        IOperationConfiguratorViewModel Create<TOperationIdentity>(IUseCase useCase, EntityName entityName, long[] operationProcessingEntities)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new();

        IOperationConfiguratorViewModel Create(IUseCase useCase, IOperationIdentity operationIdentity, EntityName entityName, long[] operationProcessingEntities);
    }
}
