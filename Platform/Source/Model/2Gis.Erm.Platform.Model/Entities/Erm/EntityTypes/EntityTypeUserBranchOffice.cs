using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeUserBranchOffice : EntityTypeBase<EntityTypeUserBranchOffice>
    {
        public override int Id
        {
            get { return EntityTypeIds.UserBranchOffice; }
        }

        public override string Description
        {
            get { return "UserBranchOffice"; }
        }
    }
}