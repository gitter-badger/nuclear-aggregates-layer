using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCharge : EntityTypeBase<EntityTypeCharge>
    {
        public override int Id
        {
            get { return EntityTypeIds.Charge; }
        }

        public override string Description
        {
            get { return "Charge"; }
        }
    }
}