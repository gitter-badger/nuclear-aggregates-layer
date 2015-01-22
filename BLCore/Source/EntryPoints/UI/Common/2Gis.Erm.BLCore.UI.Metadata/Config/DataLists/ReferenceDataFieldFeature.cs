using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class ReferenceDataFieldFeature : IDataFieldFeature
    {
        public ReferenceDataFieldFeature(string referencedPropertyName, IEntityType entityName)
        {
            ReferencedPropertyName = referencedPropertyName;
            ReferencedEntityName = entityName;
        }

        public string ReferencedPropertyName { get; set; }

        public IEntityType ReferencedEntityName { get; set; }
    }
}