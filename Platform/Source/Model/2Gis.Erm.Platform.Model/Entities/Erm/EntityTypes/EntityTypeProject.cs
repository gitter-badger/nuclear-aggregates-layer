using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeProject : EntityTypeBase<EntityTypeProject>
    {
        public override int Id
        {
            get { return EntityTypeIds.Project; }
        }

        public override string Description
        {
            get { return "Project"; }
        }
    }
}