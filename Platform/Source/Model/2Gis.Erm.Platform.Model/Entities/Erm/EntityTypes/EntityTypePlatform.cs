using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePlatform : EntityTypeBase<EntityTypePlatform>
    {
        public override int Id
        {
            get { return EntityTypeIds.Platform; }
        }

        public override string Description
        {
            get { return "Platform"; }
        }
    }
}