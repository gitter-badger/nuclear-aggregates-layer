using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation
{
    public sealed class NavigationMetadataSource : MetadataSourceBase<MetadataNavigationIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public NavigationMetadataSource()
        {
            var navigationRoot = NavigationSettings.Settings;
            _metadata = new Dictionary<Uri, IMetadataElement> { { navigationRoot.Identity.Id, navigationRoot } };
        }
        
        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}