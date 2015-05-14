using System;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.Execute
{
    public sealed class ExecuteSaveMessageHandler : UseCaseSyncMessageHandlerBase<ExecuteActionMessage>
    {
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly IViewModelMapperFactory _viewModelMapperFactory;

        public ExecuteSaveMessageHandler(IOperationServicesManager operationServicesManager, IViewModelMapperFactory viewModelMapperFactory)
        {
            _operationServicesManager = operationServicesManager;
            _viewModelMapperFactory = viewModelMapperFactory;
        }

        protected override bool ConcreteCanHandle(ExecuteActionMessage message, IUseCase useCase)
        {
            var targetOperation = message.Operation;
            return targetOperation != null 
                && targetOperation.OperationIdentity.Equals(ModifyBusinessModelEntityIdentity.Instance) 
                && !useCase.State.IsEmpty 
                && (message.Confirmed || !message.NeedConfirmation);
        }

        protected override IMessageProcessingResult ConcreteHandle(ExecuteActionMessage message, IUseCase useCase)
        {
            IViewModel viewModel;
            if (!useCase.TryGetViewModelById(message.ActionHostId, out viewModel))
            {
                return null;
            }

            var cardViewModel = viewModel as ICardViewModel<ICardViewModelIdentity>;
            if (cardViewModel == null)
            {
                return null;
            }

            var mapper = _viewModelMapperFactory.GetMapper(cardViewModel.ConcreteIdentity.EntityName);
            var service = _operationServicesManager.GetModifyDomainEntityService(cardViewModel.ConcreteIdentity.EntityName);

            var currentElement = useCase.State.Current;
            if (currentElement == null)
            {
                return null;
            }

            var currentViewModel = currentElement.Context as IViewModel;
            if (currentViewModel == null)
            {
                return null;
            }

            var dto = mapper.ToDto(useCase, cardViewModel);

            try
            {
                service.Modify(dto);
            }
            catch (Exception ex)
            {
                if (message.NeedConfirmation)
                {
                    // TODO {all, 22.07.2013}: confirmation support for createorupdate required?
                }
            }

            return message.NeedConfirmation ? null : EmptyResult;
        }
    }
}