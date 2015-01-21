using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeRole : EntityTypeBase<EntityTypeRole>
    {
        public override int Id
        {
            get { return EntityTypeIds.Role; }
        }

        public override string Description
        {
            get { return "Role"; }
        }
    }
}