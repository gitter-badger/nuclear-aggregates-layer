using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeEmiratesLegalPersonPart : EntityTypeBase<EntityTypeEmiratesLegalPersonPart>
    {
        public override int Id
        {
            get { return EntityTypeIds.EmiratesLegalPersonPart; }
        }

        public override string Description
        {
            get { return "EmiratesLegalPersonPart"; }
        }
    }
}