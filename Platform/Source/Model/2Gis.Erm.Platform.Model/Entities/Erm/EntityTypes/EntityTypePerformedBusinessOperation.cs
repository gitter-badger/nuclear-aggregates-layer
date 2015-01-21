using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePerformedBusinessOperation : EntityTypeBase<EntityTypePerformedBusinessOperation>
    {
        public override int Id
        {
            get { return EntityTypeIds.PerformedBusinessOperation; }
        }

        public override string Description
        {
            get { return "PerformedBusinessOperation"; }
        }
    }
}