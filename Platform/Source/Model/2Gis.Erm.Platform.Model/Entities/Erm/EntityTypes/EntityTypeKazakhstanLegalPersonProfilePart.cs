using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeKazakhstanLegalPersonProfilePart : EntityTypeBase<EntityTypeKazakhstanLegalPersonProfilePart>
    {
        public override int Id
        {
            get { return EntityTypeIds.KazakhstanLegalPersonProfilePart; }
        }

        public override string Description
        {
            get { return "KazakhstanLegalPersonProfilePart"; }
        }
    }
}