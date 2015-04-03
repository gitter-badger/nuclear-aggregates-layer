using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Utils;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels.Contextual
{
    public sealed class ContextualDocumentProvider : IContextualDocumentProvider
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IDocumentManager _documentManager;

        public ContextualDocumentProvider(IMetadataProvider metadataProvider, IDocumentManager documentManager)
        {
            _metadataProvider = metadataProvider;
            _documentManager = documentManager;
        }

        public void AttachContextualDocument()
        {
            IMetadataElement navigationRoot;
            if (!_metadataProvider.TryGetMetadata(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataNavigationIdentity>(), out navigationRoot))
            {
                throw new InvalidOperationException("Can't resolve navigation root metadata");
            }

            var registry = new Dictionary<Type, IViewModelViewMapping>();
            foreach (var container in navigationRoot.Elements)
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