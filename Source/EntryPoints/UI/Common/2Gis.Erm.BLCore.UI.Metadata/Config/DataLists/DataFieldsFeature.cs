using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataFieldsFeature : IConfigFeature
    {
        private readonly List<DataFieldStructure> _dataFields;

        public DataFieldsFeature(IEnumerable<DataFieldStructure> dataFields)
        {
            _dataFields = new List<DataFieldStructure>(dataFields);
        }

        public ICollection<DataFieldStructure> DataFields
        {
            get { return _dataFields; }
        }
    }
}