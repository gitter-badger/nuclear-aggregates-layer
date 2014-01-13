using System;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Resolvers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents.Contextual;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Resolvers
{
    public sealed class NavigationUsecaseResolver : UseCaseResolverBase<NavigationMessage>
    {
        private readonly IDocumentsStateInfo _documentsState;
        private readonly INavigationSettingsProvider _navigationSettingsProvider;
        private readonly Func<object, INavigationItem, IConfigElement, bool>[] _resolvers;

        public NavigationUsecaseResolver(IDocumentsStateInfo documentsState, INavigationSettingsProvider navigationSettingsProvider)
        {
            _documentsState = documentsState;
            _navigationSettingsProvider = navigationSettingsProvider;

            _resolvers =
                new Func<object, INavigationItem, IConfigElement, bool>[]
                    {
                        GridContext,
                        OtherContext
                    };
        }

        protected override bool IsAppropriate(IUseCase checkingUseCase, NavigationMessage message)
        {
            var currentElement = checkingUseCase.State.Current;
            if (checkingUseCase.State.IsEmpty
                || currentElement == null)
            {
                return false;
            }

            var currentDocuments = _documentsState.Documents;
            if (currentDocuments == null)
            {
                return false;
            }

            var contextualDocument = (IContextualDocument)currentDocuments.SingleOrDefault(document => document is IContextualDocument);
            if (contextualDocument == null)
            {
                return false;
            }

            IConfigElement targetConfigElement;
            if (!_navigationSettingsProvider.Settings.TryGetElementById(message.UsedItem.Id, out targetConfigElement))
            {
                return false;
            }

            return _resolvers.Any(resolver => resolver(currentElement.Context, message.UsedItem, targetConfigElement));
        }

        private bool GridContext(object context, INavigationItem item, IConfigElement element)
        {
            var contextualDocument = context as IContextualDocument;
            if (contextualDocument == null)
            {
                return false;
            }

            var currentContext = contextualDocument.Context;
            if (currentContext == null)
            {
                return false;
            }
            
            var gridViewModel = currentContext.Context as IGridViewModel;
            if (gridViewModel == null)
            {
                return false;
            }

            return true;
        }

        private bool OtherContext(object context, INavigationItem item, IConfigElement element)
        {
            var contextualDocument = context as IContextualDocument;
            if (contextualDocument == null)
            {
                return false;
            }

            var currentContext = contextualDocument.Context;
            if (currentContext == null)
            {
                return false;
            }

            var viewModel = currentContext.Context as NullViewModel;
            if (viewModel == null)
            {
                return false;
            }

            return true;
        }
    }
}