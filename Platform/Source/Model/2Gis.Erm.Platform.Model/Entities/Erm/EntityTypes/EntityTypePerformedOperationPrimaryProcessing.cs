using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePerformedOperationPrimaryProcessing : EntityTypeBase<EntityTypePerformedOperationPrimaryProcessing>
    {
        public override int Id
        {
            get { return EntityTypeIds.PerformedOperationPrimaryProcessing; }
        }

        public override string Description
        {
            get { return "PerformedOperationPrimaryProcessing"; }
        }
    }
}