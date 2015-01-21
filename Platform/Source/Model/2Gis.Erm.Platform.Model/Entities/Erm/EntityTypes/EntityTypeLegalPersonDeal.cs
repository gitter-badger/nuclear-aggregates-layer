using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeLegalPersonDeal : EntityTypeBase<EntityTypeLegalPersonDeal>
    {
        public override int Id
        {
            get { return EntityTypeIds.LegalPersonDeal; }
        }

        public override string Description
        {
            get { return "LegalPersonDeal"; }
        }
    }
}