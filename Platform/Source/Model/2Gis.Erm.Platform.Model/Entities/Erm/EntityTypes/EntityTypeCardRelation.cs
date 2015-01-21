using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCardRelation : EntityTypeBase<EntityTypeCardRelation>
    {
        public override int Id
        {
            get { return EntityTypeIds.CardRelation; }
        }

        public override string Description
        {
            get { return "CardRelation"; }
        }
    }
}