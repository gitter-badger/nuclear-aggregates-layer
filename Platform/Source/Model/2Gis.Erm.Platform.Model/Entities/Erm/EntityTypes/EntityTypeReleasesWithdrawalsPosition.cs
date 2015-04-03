using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeReleasesWithdrawalsPosition : EntityTypeBase<EntityTypeReleasesWithdrawalsPosition>
    {
        public override int Id
        {
            get { return EntityTypeIds.ReleasesWithdrawalsPosition; }
        }

        public override string Description
        {
            get { return "ReleasesWithdrawalsPosition"; }
        }
    }
}