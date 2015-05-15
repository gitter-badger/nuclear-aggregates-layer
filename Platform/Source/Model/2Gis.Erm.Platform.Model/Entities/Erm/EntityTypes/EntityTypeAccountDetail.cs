using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAccountDetail : EntityTypeBase<EntityTypeAccountDetail>
    {
        public override int Id
        {
            get { return EntityTypeIds.AccountDetail; }
        }

        public override string Description
        {
            get { return "AccountDetail"; }
        }
    }
}