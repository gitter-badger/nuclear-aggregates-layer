using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeEmiratesLegalPersonProfilePart : EntityTypeBase<EntityTypeEmiratesLegalPersonProfilePart>
    {
        public override int Id
        {
            get { return EntityTypeIds.EmiratesLegalPersonProfilePart; }
        }

        public override string Description
        {
            get { return "EmiratesLegalPersonProfilePart"; }
        }
    }
}