using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeDenormalizedClientLink : EntityTypeBase<EntityTypeDenormalizedClientLink>
    {
        public override int Id
        {
            get { return EntityTypeIds.DenormalizedClientLink; }
        }

        public override string Description
        {
            get { return "DenormalizedClientLink"; }
        }
    }
}