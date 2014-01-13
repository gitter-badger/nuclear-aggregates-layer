using System;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Grid;
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
    /// TODO бывший ресолвер - пытается переиспользовать контекстный документ
    /// нужно объеденить с NavigationPaneActionHandler
    public sealed class NavigationPaneActionHandler2 : UseCaseSyncMessageHandlerBase<NavigationMessage>
    {
        private readonly IGridViewModelFactory _gridViewModelFactory;
        private readonly IDocumentsStateInfo _documentsState;
        private readonly INavigationSettingsProvider _navigationSettingsProvider;
        private readonly Func<IUseCase, object, INavigationItem, IConfigElement, IContextualDocumentContext>[] _resolvers;

        public NavigationPaneActionHandler2(
            IGridViewModelFactory gridViewModelFactory,
            IDocumentsStateInfo documentsState, 
            INavigationSettingsProvider navigationSettingsProvider)
        {
            _gridViewModelFactory = gridViewModelFactory;
            _documentsState = documentsState;
            _navigationSettingsProvider = navigationSettingsProvider;

            _resolvers = new Func<IUseCase, object, INavigationItem, IConfigElement, IContextualDocumentContext>[] { GridContext, OtherContext };
        }

        protected override bool ConcreteCanHandle(NavigationMessage message, IUseCase useCase)
        {
            return !useCase.State.IsEmpty && useCase.State.Root.IsDegenerated;
        }

        protected override IMessageProcessingResult ConcreteHandle(NavigationMessage message, IUseCase useCase)
        {
            var currentElement = useCase.State.Current;
            if (currentElement == null)
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

            foreach (var resolver in _resolvers)
            {
                var resolvedContext = resolver(useCase, currentElement.Context, message.UsedItem, targetConfigElement);
                if (resolvedContext != null)
                {
                    return EmptyResult;
                }
            }

            return null;
        }

        private IContextualDocumentContext GridContext(IUseCase useCase, object context, INavigationItem item, IConfigElement element)
        {
            var contextualDocument = context as IContextualDocument;
            if (contextualDocument == null)
            {
                return null;
            }

            var currentContext = contextualDocument.Context;
            if (currentContext == null)
            {
                return null;
            }
            
            var oldGridViewModel = currentContext.Context as IGridViewModel;
            if (oldGridViewModel == null)
            {
                return null;
            }

            var mappingFeature = element.ElementFeatures.OfType<IViewModelViewMappingFeature>().SingleOrDefault();
            if (mappingFeature == null)
            {
                return null;
            }

            if (mappingFeature.Mapping.ViewModelType != typeof(GridViewModel))
            {
                return null;
            }

            var handlerBoundElement = element as IHandlerBoundElement;
            if (handlerBoundElement == null || !handlerBoundElement.HasHandler)
            {
                return null;
            }

            var showGridDetail = handlerBoundElement.Handler as ShowGridHandlerFeature;
            if (showGridDetail == null)
            {
                return null;
            }

            var gridViewModel = _gridViewModelFactory.Create(useCase, showGridDetail.EntityName);

            var newContext = new ContextualDocumentContext { Title = item.Title, Context = gridViewModel };
            contextualDocument.Context = newContext;

            var disposableOldGridViewModel = oldGridViewModel as IDisposable;
            if (disposableOldGridViewModel != null)
            {
                disposableOldGridViewModel.Dispose();
            }

            return newContext;
        }

        private IContextualDocumentContext OtherContext(IUseCase useCase, object context, INavigationItem item, IConfigElement element)
        {
            var contextualDocument = context as IContextualDocument;
            if (contextualDocument == null)
            {
                return null;
            }

            var currentContext = contextualDocument.Context;
            if (currentContext == null)
            {
                return null;
            }

            var viewModel = currentContext.Context as NullViewModel;
            if (viewModel == null)
            {
                return null;
            }


            var mappingFeature = element.ElementFeatures.OfType<IViewModelViewMappingFeature>().SingleOrDefault();
            if (mappingFeature == null)
            {
                return null;
            }

            if (mappingFeature.Mapping.ViewModelType != typeof(NullViewModel))
            {
                return null;
            }

            viewModel.Context = item.Title;
            var newContext = new ContextualDocumentContext { Title = item.Title, Context = viewModel };
            contextualDocument.Context = newContext;
            return newContext;
        }
    }
}