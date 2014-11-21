using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    public sealed class OperationProgressMessageHandler : UseCaseSyncMessageHandlerBase<OperationProgressMessage>
    {
        protected override IMessageProcessingResult ConcreteHandle(OperationProgressMessage message, IUseCase useCase)
        {
            var processings = useCase.State.ProcessingsRegistry.Processings;
            if (processings == null)
            {
                return null;
            }

            var targetOperationProcessing = processings.SingleOrDefault(p => message.OperationToken == p.Id) as IOperationProcessingDescriptor;
            if (targetOperationProcessing == null)
            {
                return null;
            }

            targetOperationProcessing.UpdateOperationProgress(message.Results);

            if (!targetOperationProcessing.ProgressConsumerId.HasValue)
            {
                return EmptyResult;
            }

            IViewModel viewModel;
            if (!useCase.TryGetViewModelById(targetOperationProcessing.ProgressConsumerId.Value, out viewModel) || viewModel == null)
            {
                return EmptyResult;
            }

            var operationConfigurator = viewModel as IOperationConfiguratorViewModel;
            if (operationConfigurator == null)
            {
                return EmptyResult;
            }

            operationConfigurator.UpdateOperationProgress(message.Results);
            return EmptyResult;
        }
    }
}