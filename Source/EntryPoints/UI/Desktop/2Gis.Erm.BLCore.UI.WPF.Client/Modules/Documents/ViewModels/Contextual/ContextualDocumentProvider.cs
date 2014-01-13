using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Utils;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels.Contextual
{
    public sealed class ContextualDocumentProvider : IContextualDocumentProvider
    {
        private readonly INavigationSettingsProvider _navigationSettingsProvider;
        private readonly IDocumentManager _documentManager;

        public ContextualDocumentProvider(INavigationSettingsProvider navigationSettingsProvider, IDocumentManager documentManager)
        {
            _navigationSettingsProvider = navigationSettingsProvider;
            _documentManager = documentManager;
        }

        public void AttachContextualDocument()
        {
            var navigationSettings = _navigationSettingsProvider.Settings;
            var registry = new Dictionary<Type, IViewModelViewMapping>();
            foreach (var container in navigationSettings)
            {
                if (container.Elements == null || !container.Elements.Any())
                {
                    continue;
                }

                foreach (var element in container.Elements)
                {
                    element.ProcessMVVMMappings(registry);
                }
            }

            var selector = new ViewModel2ViewMappingsSelector(registry.Values);
            var contextualDocument = new ContextualDocumentViewModel(selector);
            _documentManager.Add(contextualDocument);
        }
    }
}