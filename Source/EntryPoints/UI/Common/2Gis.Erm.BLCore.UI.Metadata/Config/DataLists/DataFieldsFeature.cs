using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataFieldsFeature : IMetadataFeature
    {
        private readonly List<DataFieldMetadata> _dataFields;

        public DataFieldsFeature(IEnumerable<DataFieldMetadata> dataFields)
        {
            _dataFields = new List<DataFieldMetadata>(dataFields);
        }

        public ICollection<DataFieldMetadata> DataFields
        {
            get { return _dataFields; }
        }
    }
}