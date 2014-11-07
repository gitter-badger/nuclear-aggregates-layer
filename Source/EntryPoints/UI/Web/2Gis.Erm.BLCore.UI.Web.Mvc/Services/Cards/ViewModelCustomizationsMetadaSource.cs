using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class ViewModelCustomizationsMetadaSource : MetadataSourceBase<ViewModelCustomizationsIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public ViewModelCustomizationsMetadaSource()
        {
            _metadata = InitializeMetadataContainer();
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }

        private static IReadOnlyDictionary<Uri, IMetadataElement> InitializeMetadataContainer()
        {
            IReadOnlyCollection<ViewModelCustomizationsMetada> metadataContainer =
                new ViewModelCustomizationsMetada[]
                    {
                    };

            return metadataContainer.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }
    }
}
