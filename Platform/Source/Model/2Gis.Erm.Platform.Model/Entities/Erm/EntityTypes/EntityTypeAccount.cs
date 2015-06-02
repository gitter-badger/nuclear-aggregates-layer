using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAccount : EntityTypeBase<EntityTypeAccount>
    {
        public override int Id
        {
            get { return EntityTypeIds.Account; }
        }

        public override string Description
        {
            get { return "Account"; }
        }
    }
}