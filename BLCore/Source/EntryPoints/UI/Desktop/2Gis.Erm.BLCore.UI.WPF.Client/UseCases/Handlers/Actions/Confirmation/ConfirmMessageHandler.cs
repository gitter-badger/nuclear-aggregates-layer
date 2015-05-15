using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations;
using DoubleGis.Erm.Platform.Resources.Client;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.Confirmation
{
    public sealed class ConfirmMessageHandler : UseCaseSyncMessageHandlerBase<ExecuteActionMessage>
    {
        private readonly IOperationConfiguratorViewModelFactory _operationConfiguratorViewModelFactory;
        private readonly IDocumentManager _documentManager;
        private readonly List<Func<IViewModel, Tuple<IEntityType, long[]>>> _resolvers;

        public ConfirmMessageHandler(IOperationConfiguratorViewModelFactory operationConfiguratorViewModelFactory, IDocumentManager documentManager)
        {
            _operationConfiguratorViewModelFactory = operationConfiguratorViewModelFactory;
            _documentManager = documentManager;
            _resolvers = new List<Func<IViewModel, Tuple<IEntityType, long[]>>> { FromCard, FromGrid };
        }

        protected override bool ConcreteCanHandle(ExecuteActionMessage message, IUseCase useCase)
        {   
            return !useCase.State.IsEmpty && message.NeedConfirmation && !message.Confirmed;
        }

        protected override IMessageProcessingResult ConcreteHandle(ExecuteActionMessage message, IUseCase useCase)
        {
            var currentElement = useCase.State.Current;
            if (currentElement == null)
            {
                return null;
            }
            
            var targetOperation = message.Operation;
            if (targetOperation == null)
            {
                return null;
            }

            IViewModel viewModel;
            if (!useCase.TryGetViewModelById(message.ActionHostId, out viewModel))
            {
                return null;
            }

            Tuple<IEntityType, long[]> operationParameters = null;
            foreach (var resolver in _resolvers)
            {
                operationParameters = resolver(viewModel);
            }

            if (operationParameters == null)
            {
                return null;
            }

            var operationManagerViewModel = _operationConfiguratorViewModelFactory.Create(useCase,
                                                                                          targetOperation.OperationIdentity,
                                                                                          operationParameters.Item1,
                                                                                          operationParameters.Item2);
            if (operationManagerViewModel == null)
            {
                return null;
            }

            if (!useCase.State.TryMoveNext(operationManagerViewModel))
            {
                throw new InvalidOperationException(ResPlatformUI.ErrorDetectedWhenChangingUseCaseState);
            }

            var documents = useCase.State.NodesSnapshot.Select(x => x.Context).OfType<IDocument>();
            foreach (var document in documents)
            {
                _documentManager.AddDialog(document, operationManagerViewModel);
            }

            return EmptyResult;
        }

        private Tuple<IEntityType, long[]> FromCard(IViewModel viewModel)
        {
            var cardViewModel = viewModel as ICardViewModel<ICardViewModelIdentity>;
            return cardViewModel != null 
                ? new Tuple<IEntityType, long[]>(cardViewModel.ConcreteIdentity.EntityName, new[] { cardViewModel.ConcreteIdentity.EntityId }) 
                : null;
        }

        private Tuple<IEntityType, long[]> FromGrid(IViewModel viewModel)
        {
            var gridViewModel = viewModel as IGridViewModel<IGridViewModelIdentity>;
            if (gridViewModel == null)
            {
                return null;
            }

            var gridSelectedItems = gridViewModel.SelectedItems;
            var items = gridSelectedItems != null
                            ? gridViewModel.SelectedItems.Select(item => ViewModelUtils.ExtractPropertyValue<long>(item, "Id")).ToArray()
                            : new long[0];

            return new Tuple<IEntityType, long[]>(gridViewModel.ConcreteIdentity.EntityName, items);
        }
    }
}