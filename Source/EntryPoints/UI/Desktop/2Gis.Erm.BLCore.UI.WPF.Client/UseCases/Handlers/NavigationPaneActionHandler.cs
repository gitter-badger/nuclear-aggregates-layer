using System;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler.Concrete;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents.Contextual;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    /// TODO бывший launcher - создает заново контекстный документ
    /// нужно объеденить с NavigationPaneActionHandler2
    public sealed class NavigationPaneActionHandler : UseCaseSyncMessageHandlerBase<NavigationMessage>
    {
        private readonly IGridViewModelFactory _gridViewModelFactory;
        private readonly IDocumentsStateInfo _documentsState;
        private readonly INavigationSettingsProvider _navigationSettingsProvider;
        private readonly Func<IUseCase, INavigationItem, IConfigElement, IContextualDocumentContext>[] _contextResolvers;

        public NavigationPaneActionHandler(
            IGridViewModelFactory gridViewModelFactory,
            IDocumentsStateInfo documentsState, 
            INavigationSettingsProvider navigationSettingsProvider)
        {
            _gridViewModelFactory = gridViewModelFactory;
            _documentsState = documentsState;
            _navigationSettingsProvider = navigationSettingsProvider;

            _contextResolvers =
                new Func<IUseCase, INavigationItem, IConfigElement, IContextualDocumentContext>[] { GridContext, OtherContext };
        }

        protected override bool ConcreteCanHandle(NavigationMessage message, IUseCase useCase)
        {
            if (!useCase.State.IsEmpty)
            {
                return false;
            }

            var currentDocuments = _documentsState.Documents;
            if (currentDocuments == null)
            {
                return false;
            }

            return currentDocuments.SingleOrDefault(document => document is IContextualDocument) != null;
        }

        protected override IMessageProcessingResult ConcreteHandle(NavigationMessage message, IUseCase useCase)
        {
            if (!useCase.State.IsEmpty)
            {
                return null;
            }

            var currentDocuments = _documentsState.Documents;
            if (currentDocuments == null)
            {
                return null;
            }

            var contextualDocument = (IContextualDocument)currentDocuments.SingleOrDefault(document => document is IContextualDocument);
            if (contextualDocument == null)
            {
                return null;
            }

            IConfigElement targetConfigElement;
            if (!_navigationSettingsProvider.Settings.TryGetElementById(message.UsedItem.Id, out targetConfigElement))
            {
                return null;
            }

            IContextualDocumentContext resolvedContext = null;
            foreach (var contextResolver in _contextResolvers)
            {
                resolvedContext = contextResolver(useCase, message.UsedItem, targetConfigElement);
                if (resolvedContext != null)
                {
                    break;
                }
            }

            if (resolvedContext == null)
            {
                // TODO подумать что делать с таким неудавшимся useCase useCase.Dispose();
                return null;
            }

            if (!useCase.State.TryMoveNext(contextualDocument))
            {
                throw new InvalidOperationException(BLCore.Resources.Client.Properties.Resources.ErrorDetectedWhenChangingUseCaseState);
            }

            contextualDocument.Context = resolvedContext;
            
            return EmptyResult;
        }

        private IContextualDocumentContext GridContext(IUseCase useCase, INavigationItem item, IConfigElement element)
        {
            var handlerBoundElement = element as IHandlerBoundElement;
            if (handlerBoundElement == null || !handlerBoundElement.HasHandler)
            {
                return null;
            }

            var showGridHandler = handlerBoundElement.Handler as ShowGridHandlerFeature;
            if (showGridHandler == null)
            {
                return null;
            }

            var mvvmForGridFeature = element.ElementFeatures.OfType<IViewModelViewMappingFeature>().SingleOrDefault();
            if (mvvmForGridFeature == null)
            {
                return null;
            }

            var gridViewModel = _gridViewModelFactory.Create(useCase, showGridHandler.EntityName);
            return new ContextualDocumentContext { Title = item.Title, Context = gridViewModel };
        }

        private IContextualDocumentContext OtherContext(IUseCase useCase, INavigationItem item, IConfigElement element)
        {
            return new ContextualDocumentContext { Title = item.Title, Context = new NullViewModel { Context = item.Title } };
        }
    }
}
