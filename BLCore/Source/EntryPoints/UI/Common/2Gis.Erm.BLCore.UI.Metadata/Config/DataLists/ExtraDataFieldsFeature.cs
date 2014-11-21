using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class ExtraDataFieldsFeature : IDataFieldFeature
    {
        private readonly List<DataFieldMetadata> _extraDataFields;

        public ExtraDataFieldsFeature(params DataFieldMetadata[] extraDataFields)
        {
            _extraDataFields = new List<DataFieldMetadata>(extraDataFields);
        }

        public ICollection<DataFieldMetadata> ExtraDataFields
        {
            get { return _extraDataFields; }
        }
    }
}