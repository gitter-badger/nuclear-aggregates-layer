using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public class AttachedDataListsFeature : IMetadataFeature
    {
        private readonly List<DataListMetadata> _dataLists;

        public AttachedDataListsFeature(IEnumerable<DataListMetadata> dataLists)
        {
            _dataLists = new List<DataListMetadata>(dataLists);
        }

        public ICollection<DataListMetadata> DataLists
        {
            get { return _dataLists; }
        }
    }
}