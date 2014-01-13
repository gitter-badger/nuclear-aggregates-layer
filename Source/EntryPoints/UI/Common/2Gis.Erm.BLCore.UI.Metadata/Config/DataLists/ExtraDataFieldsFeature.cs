using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class ExtraDataFieldsFeature : IDataFieldFeature
    {
        private readonly List<DataFieldStructure> _extraDataFields;

        public ExtraDataFieldsFeature(params DataFieldStructure[] extraDataFields)
        {
            _extraDataFields = new List<DataFieldStructure>(extraDataFields);
        }

        public ICollection<DataFieldStructure> ExtraDataFields
        {
            get { return _extraDataFields; }
        }
    }
}