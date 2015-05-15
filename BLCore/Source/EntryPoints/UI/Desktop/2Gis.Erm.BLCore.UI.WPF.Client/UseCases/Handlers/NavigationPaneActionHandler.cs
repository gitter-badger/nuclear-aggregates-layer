using System;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.Resources.Client;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents.Contextual;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

using NuClear.Metamodeling.Domain.Elements.Aspects.Features.Handler;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Handler.Concrete;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    // FIXME {all, 17.04.2014}: бывший launcher - создает заново контекстный документ - нужно объеденить с NavigationPaneActionHandler2
    public sealed class NavigationPaneActionHandler : UseCaseSyncMessageHandlerBase<NavigationMessage>
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IGridViewModelFactory _gridViewModelFactory;
        private readonly IDocumentsStateInfo _documentsState;
        private readonly Func<IUseCase, INavigationItem, IMetadataElement, IContextualDocumentContext>[] _contextResolvers;

        public NavigationPaneActionHandler(
            IMetadataProvider metadataProvider,
            IGridViewModelFactory gridViewModelFactory,
            IDocumentsStateInfo documentsState)
        {
            _metadataProvider = metadataProvider;
            _gridViewModelFactory = gridViewModelFactory;
            _documentsState = documentsState;

            _contextResolvers =
                new Func<IUseCase, INavigationItem, IMetadataElement, IContextualDocumentContext>[] { GridContext, OtherContext };
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

            IMetadataElement navigationElementMetadata;
            if (!_metadataProvider.TryGetMetadata(message.UsedItem.Id, out navigationElementMetadata))
            {
                return null;
            }

            IContextualDocumentContext resolvedContext = null;
            foreach (var contextResolver in _contextResolvers)
            {
                resolvedContext = contextResolver(useCase, message.UsedItem, navigationElementMetadata);
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
                throw new InvalidOperationException(ResPlatformUI.ErrorDetectedWhenChangingUseCaseState);
            }

            contextualDocument.Context = resolvedContext;
            
            return EmptyResult;
        }

        private IContextualDocumentContext GridContext(IUseCase useCase, INavigationItem item, IMetadataElement element)
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

            var mvvmForGridFeature = element.Features.OfType<IViewModelViewMappingFeature>().SingleOrDefault();
            if (mvvmForGridFeature == null)
            {
                return null;
            }

            var gridViewModel = _gridViewModelFactory.Create(useCase, showGridHandler.EntityName);
            return new ContextualDocumentContext { Title = item.Title, Context = gridViewModel };
        }

        private IContextualDocumentContext OtherContext(IUseCase useCase, INavigationItem item, IMetadataElement element)
        {
            return new ContextualDocumentContext { Title = item.Title, Context = new NullViewModel { Context = item.Title } };
        }
    }
}
