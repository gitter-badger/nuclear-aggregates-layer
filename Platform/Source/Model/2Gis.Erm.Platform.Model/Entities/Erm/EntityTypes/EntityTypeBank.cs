using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBank : EntityTypeBase<EntityTypeBank>
    {
        public override int Id
        {
            get { return EntityTypeIds.Bank; }
        }

        public override string Description
        {
            get { return "Bank"; }
        }
    }
}