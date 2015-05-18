using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeWithdrawalInfo : EntityTypeBase<EntityTypeWithdrawalInfo>
    {
        public override int Id
        {
            get { return EntityTypeIds.WithdrawalInfo; }
        }

        public override string Description
        {
            get { return "WithdrawalInfo"; }
        }
    }
}