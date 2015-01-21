using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePosition : EntityTypeBase<EntityTypePosition>
    {
        public override int Id
        {
            get { return EntityTypeIds.Position; }
        }

        public override string Description
        {
            get { return "Position"; }
        }
    }
}