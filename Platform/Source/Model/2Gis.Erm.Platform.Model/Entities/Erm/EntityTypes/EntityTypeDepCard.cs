using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeDepCard : EntityTypeBase<EntityTypeDepCard>
    {
        public override int Id
        {
            get { return EntityTypeIds.DepCard; }
        }

        public override string Description
        {
            get { return "DepCard"; }
        }
    }
}