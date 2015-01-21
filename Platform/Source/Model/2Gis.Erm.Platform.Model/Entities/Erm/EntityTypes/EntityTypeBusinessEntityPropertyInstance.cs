using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBusinessEntityPropertyInstance : EntityTypeBase<EntityTypeBusinessEntityPropertyInstance>
    {
        public override int Id
        {
            get { return EntityTypeIds.BusinessEntityPropertyInstance; }
        }

        public override string Description
        {
            get { return "BusinessEntityPropertyInstance"; }
        }
    }
}