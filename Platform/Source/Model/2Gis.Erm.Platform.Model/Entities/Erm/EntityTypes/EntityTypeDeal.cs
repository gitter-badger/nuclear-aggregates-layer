using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeDeal : EntityTypeBase<EntityTypeDeal>
    {
        public override int Id
        {
            get { return EntityTypeIds.Deal; }
        }

        public override string Description
        {
            get { return "Deal"; }
        }
    }
}