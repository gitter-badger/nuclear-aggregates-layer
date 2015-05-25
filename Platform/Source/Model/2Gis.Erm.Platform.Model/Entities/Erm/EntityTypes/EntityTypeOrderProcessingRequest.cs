using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOrderProcessingRequest : EntityTypeBase<EntityTypeOrderProcessingRequest>
    {
        public override int Id
        {
            get { return EntityTypeIds.OrderProcessingRequest; }
        }

        public override string Description
        {
            get { return "OrderProcessingRequest"; }
        }
    }
}