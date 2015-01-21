using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeLegalPersonProfile : EntityTypeBase<EntityTypeLegalPersonProfile>
    {
        public override int Id
        {
            get { return EntityTypeIds.LegalPersonProfile; }
        }

        public override string Description
        {
            get { return "LegalPersonProfile"; }
        }
    }
}