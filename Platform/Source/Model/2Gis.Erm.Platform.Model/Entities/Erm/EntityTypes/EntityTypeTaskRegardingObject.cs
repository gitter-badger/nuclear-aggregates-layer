using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeTaskRegardingObject : EntityTypeBase<EntityTypeTaskRegardingObject>
    {
        public override int Id
        {
            get { return EntityTypeIds.TaskRegardingObject; }
        }

        public override string Description
        {
            get { return "TaskRegardingObject"; }
        }
    }
}