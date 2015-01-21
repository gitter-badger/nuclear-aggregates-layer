using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeEmiratesFirmAddressPart : EntityTypeBase<EntityTypeEmiratesFirmAddressPart>
    {
        public override int Id
        {
            get { return EntityTypeIds.EmiratesFirmAddressPart; }
        }

        public override string Description
        {
            get { return "EmiratesFirmAddressPart"; }
        }
    }
}