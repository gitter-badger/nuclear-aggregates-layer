using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeImportedFirmAddress : EntityTypeBase<EntityTypeImportedFirmAddress>
    {
        public override int Id
        {
            get { return EntityTypeIds.ImportedFirmAddress; }
        }

        public override string Description
        {
            get { return "ImportedFirmAddress"; }
        }
    }
}