using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBill : EntityTypeBase<EntityTypeBill>
    {
        public override int Id
        {
            get { return EntityTypeIds.Bill; }
        }

        public override string Description
        {
            get { return "Bill"; }
        }
    }
}