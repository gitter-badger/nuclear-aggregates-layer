using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBusinessEntityInstance : EntityTypeBase<EntityTypeBusinessEntityInstance>
    {
        public override int Id
        {
            get { return EntityTypeIds.BusinessEntityInstance; }
        }

        public override string Description
        {
            get { return "BusinessEntityInstance"; }
        }
    }
}