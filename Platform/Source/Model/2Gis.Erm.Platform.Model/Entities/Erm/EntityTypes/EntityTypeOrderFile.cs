using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOrderFile : EntityTypeBase<EntityTypeOrderFile>
    {
        public override int Id
        {
            get { return EntityTypeIds.OrderFile; }
        }

        public override string Description
        {
            get { return "OrderFile"; }
        }
    }
}