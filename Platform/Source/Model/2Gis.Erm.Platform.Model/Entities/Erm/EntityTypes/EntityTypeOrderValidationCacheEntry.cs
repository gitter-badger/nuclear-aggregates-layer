using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOrderValidationCacheEntry : EntityTypeBase<EntityTypeOrderValidationCacheEntry>
    {
        public override int Id
        {
            get { return EntityTypeIds.OrderValidationCacheEntry; }
        }

        public override string Description
        {
            get { return "OrderValidationCacheEntry"; }
        }
    }
}