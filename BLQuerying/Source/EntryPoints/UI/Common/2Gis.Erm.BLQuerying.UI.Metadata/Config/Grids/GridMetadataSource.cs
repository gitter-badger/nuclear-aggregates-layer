using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids;
using DoubleGis.Erm.BLQuerying.UI.Metadata.Config.Grids.Settings;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLQuerying.UI.Metadata.Config.Grids
{
    public sealed class GridMetadataSource : MetadataSourceBase<MetadataGridsIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public GridMetadataSource()
        {
            _metadata = GridStructures.Settings.Aggregate(new Dictionary<Uri, IMetadataElement>(), Process);
        }
        
        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }

        private Dictionary<Uri, IMetadataElement> Process(Dictionary<Uri, IMetadataElement> metadata, GridMetadata element)
        {
            metadata.Add(element.Identity.Id, element);

            return metadata;
        }
    }
}