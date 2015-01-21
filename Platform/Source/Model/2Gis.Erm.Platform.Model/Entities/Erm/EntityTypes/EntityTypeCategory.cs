using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCategory : EntityTypeBase<EntityTypeCategory>
    {
        public override int Id
        {
            get { return EntityTypeIds.Category; }
        }

        public override string Description
        {
            get { return "Category"; }
        }
    }
}