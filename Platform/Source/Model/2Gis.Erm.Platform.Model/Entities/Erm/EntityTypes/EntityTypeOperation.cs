using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOperation : EntityTypeBase<EntityTypeOperation>
    {
        public override int Id
        {
            get { return EntityTypeIds.Operation; }
        }

        public override string Description
        {
            get { return "Operation"; }
        }
    }
}