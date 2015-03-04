using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.Execute
{
    public abstract class OperationMessageHandlerBase<TOperationIdentity, TCommonOperationParameter, TOperationParameter, TOperationResult> : 
        UseCaseSyncMessageHandlerBase<ExecuteActionMessage>
        where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new() 
        where TCommonOperationParameter : class, ICommonOperationParameter 
        where TOperationParameter : class, IOperationParameter
        where TOperationResult : class, IOperationResult
    {
        private readonly TOperationIdentity _operationIdentity = new TOperationIdentity();
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly ITracer _logger;

        protected OperationMessageHandlerBase(IOperationServicesManager operationServicesManager, ITracer logger)
        {
            _operationServicesManager = operationServicesManager;
            _logger = logger;
        }

        protected IOperationServicesManager OperationServicesManager
        {
            get { return _operationServicesManager; }
        }

        protected ITracer Logger
        {
            get { return _logger; }
        }

        protected TOperationIdentity OperationIdentity
        {
            get { return _operationIdentity; }
        }

        protected override bool ConcreteCanHandle(ExecuteActionMessage message, IUseCase useCase)
        {
            var targetOperation = message.Operation;
            return targetOperation != null
                && targetOperation.OperationIdentity.Equals(_operationIdentity)
                && !useCase.State.IsEmpty
                && (!message.NeedConfirmation || message.Confirmed);
        }

        protected override IMessageProcessingResult ConcreteHandle(ExecuteActionMessage message, IUseCase useCase)
        {
            IViewModel viewModel;
            if (!useCase.TryGetViewModelById(message.ActionHostId, out viewModel))
            {
                return null;
            }

            var operationManagerViewModel = viewModel as IOperationConfiguratorViewModel<TOperationIdentity, TCommonOperationParameter, TOperationParameter>;
            if (operationManagerViewModel == null)
            {
                return null;
            }

            var commonParameter = operationManagerViewModel.CommonParameter;
            var parameters = operationManagerViewModel.Parameters.ToArray();

            var operationToken = Guid.NewGuid();
            useCase.State.ProcessingsRegistry.StartProcessing(new OperationProcessingDescriptor(viewModel.Identity.Id, parameters.Length, viewModel.Identity.Id));
            
            TOperationResult[] results = new TOperationResult[0];

            try
            {
                results = ExecuteOperation(commonParameter, parameters);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Can't execute operation " + OperationIdentity);
            }
            finally
            {
                operationManagerViewModel.Finished(results);
                useCase.State.ProcessingsRegistry.FinishProcessing(operationToken);
            }

            return EmptyResult;
        }

        protected abstract TOperationResult[] ExecuteOperation(TCommonOperationParameter commonParameter, TOperationParameter[] parameters);
    }
}