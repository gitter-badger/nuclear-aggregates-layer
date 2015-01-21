using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOrderProcessingRequestMessage : EntityTypeBase<EntityTypeOrderProcessingRequestMessage>
    {
        public override int Id
        {
            get { return EntityTypeIds.OrderProcessingRequestMessage; }
        }

        public override string Description
        {
            get { return "OrderProcessingRequestMessage"; }
        }
    }
}