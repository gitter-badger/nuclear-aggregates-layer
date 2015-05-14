using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.CanExecute
{
    public sealed class CanExecuteSaveMessageHandler : UseCaseSyncMessageHandlerBase<CanExecuteActionMessage>
    {
        private readonly IOperationsMetadataProvider _operationsMetadataProvider;

        public CanExecuteSaveMessageHandler(IOperationsMetadataProvider operationsMetadataProvider)
        {
            _operationsMetadataProvider = operationsMetadataProvider;
        }

        protected override bool ConcreteCanHandle(CanExecuteActionMessage message, IUseCase useCase)
        {
            var targetOperation = message.Operation;
            return targetOperation != null && targetOperation.OperationIdentity.Equals(ModifyBusinessModelEntityIdentity.Instance) && !useCase.State.IsEmpty;
        }

        protected override IMessageProcessingResult ConcreteHandle(CanExecuteActionMessage message, IUseCase useCase)
        {
            var applicableOperations = _operationsMetadataProvider.GetApplicableOperationsForCallingUser();
            IViewModel viewModel;
            if (!useCase.TryGetViewModelById(message.ActionHostId, out viewModel))
            {
                return CanExecuteResult.False;
            }

            IEntityType entityName;
            if (!viewModel.TryGetBoundEntityName(out entityName))
            {
                return CanExecuteResult.False;
            }

            var operationEntitiesDescriptor = new EntitySet(entityName);

            var operationIdentity = SimplifiedEntities.Entities.Contains(entityName)
                                        ? (IOperationIdentity)new ModifySimplifiedModelEntityIdentity()
                                        : (IOperationIdentity)new ModifyBusinessModelEntityIdentity();

            return new MessageProcessingResult<bool>(applicableOperations
                .Any(x => x.OperationIdentity.Equals(operationIdentity) && x.MetadataDetails.Keys.Contains(operationEntitiesDescriptor)));
        }
    }
}