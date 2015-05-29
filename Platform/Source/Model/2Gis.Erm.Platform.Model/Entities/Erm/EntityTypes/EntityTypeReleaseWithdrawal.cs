using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeReleaseWithdrawal : EntityTypeBase<EntityTypeReleaseWithdrawal>
    {
        public override int Id
        {
            get { return EntityTypeIds.ReleaseWithdrawal; }
        }

        public override string Description
        {
            get { return "ReleaseWithdrawal"; }
        }
    }
}