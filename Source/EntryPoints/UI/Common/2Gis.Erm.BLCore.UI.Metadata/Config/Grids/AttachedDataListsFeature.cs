using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public class AttachedDataListsFeature : IConfigFeature
    {
        private readonly List<DataListStructure> _dataLists;

        public AttachedDataListsFeature(IEnumerable<DataListStructure> dataLists)
        {
            _dataLists = new List<DataListStructure>(dataLists);
        }

        public ICollection<DataListStructure> DataLists
        {
            get { return _dataLists; }
        }
    }
}