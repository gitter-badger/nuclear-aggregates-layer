using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFailedEntity : EntityTypeBase<EntityTypeExportFailedEntity>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFailedEntity; }
        }

        public override string Description
        {
            get { return "ExportFailedEntity"; }
        }
    }
}