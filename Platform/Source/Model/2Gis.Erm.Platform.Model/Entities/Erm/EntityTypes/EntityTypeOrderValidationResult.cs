using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOrderValidationResult : EntityTypeBase<EntityTypeOrderValidationResult>
    {
        public override int Id
        {
            get { return EntityTypeIds.OrderValidationResult; }
        }

        public override string Description
        {
            get { return "OrderValidationResult"; }
        }
    }
}