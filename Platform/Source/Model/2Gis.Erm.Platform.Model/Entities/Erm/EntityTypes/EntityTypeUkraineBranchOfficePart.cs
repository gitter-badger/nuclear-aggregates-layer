using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeUkraineBranchOfficePart : EntityTypeBase<EntityTypeUkraineBranchOfficePart>
    {
        public override int Id
        {
            get { return EntityTypeIds.UkraineBranchOfficePart; }
        }

        public override string Description
        {
            get { return "UkraineBranchOfficePart"; }
        }
    }
}