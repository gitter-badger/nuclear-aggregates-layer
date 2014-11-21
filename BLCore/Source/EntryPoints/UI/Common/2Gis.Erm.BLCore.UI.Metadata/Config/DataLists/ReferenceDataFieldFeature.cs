using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class ReferenceDataFieldFeature : IDataFieldFeature
    {
        public ReferenceDataFieldFeature(string referencedPropertyName, EntityName entityName)
        {
            ReferencedPropertyName = referencedPropertyName;
            ReferencedEntityName = entityName;
        }

        public string ReferencedPropertyName { get; set; }

        public EntityName ReferencedEntityName { get; set; }
    }
}