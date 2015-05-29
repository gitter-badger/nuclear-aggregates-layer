using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeTask : EntityTypeBase<EntityTypeTask>
    {
        public override int Id
        {
            get { return EntityTypeIds.Task; }
        }

        public override string Description
        {
            get { return "Task"; }
        }
    }
}