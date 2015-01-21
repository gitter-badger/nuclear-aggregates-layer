using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeActivity : EntityTypeBase<EntityTypeActivity>
    {
        public override int Id
        {
            get { return EntityTypeIds.Activity; }
        }

        public override string Description
        {
            get { return "Activity"; }
        }
    }
}