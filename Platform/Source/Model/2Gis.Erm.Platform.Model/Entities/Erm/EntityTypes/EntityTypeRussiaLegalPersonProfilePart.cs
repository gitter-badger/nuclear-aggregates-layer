using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeRussiaLegalPersonProfilePart : EntityTypeBase<EntityTypeRussiaLegalPersonProfilePart>
    {
        public override int Id
        {
            get { return EntityTypeIds.RussiaLegalPersonProfilePart; }
        }

        public override string Description
        {
            get { return "RussiaLegalPersonProfilePart"; }
        }
    }
}