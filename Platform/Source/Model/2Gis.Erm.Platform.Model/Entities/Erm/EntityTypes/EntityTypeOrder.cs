using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOrder : EntityTypeBase<EntityTypeOrder>
    {
        public override int Id
        {
            get { return EntityTypeIds.Order; }
        }

        public override string Description
        {
            get { return "Order"; }
        }
    }
}