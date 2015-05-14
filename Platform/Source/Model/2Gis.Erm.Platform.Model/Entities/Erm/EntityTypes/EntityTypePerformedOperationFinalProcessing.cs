using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePerformedOperationFinalProcessing : EntityTypeBase<EntityTypePerformedOperationFinalProcessing>
    {
        public override int Id
        {
            get { return EntityTypeIds.PerformedOperationFinalProcessing; }
        }

        public override string Description
        {
            get { return "PerformedOperationFinalProcessing"; }
        }
    }
}