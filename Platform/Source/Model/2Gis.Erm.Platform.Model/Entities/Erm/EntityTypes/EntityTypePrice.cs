using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePrice : EntityTypeBase<EntityTypePrice>
    {
        public override int Id
        {
            get { return EntityTypeIds.Price; }
        }

        public override string Description
        {
            get { return "Price"; }
        }
    }
}