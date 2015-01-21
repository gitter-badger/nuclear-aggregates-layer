using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCategoryGroup : EntityTypeBase<EntityTypeCategoryGroup>
    {
        public override int Id
        {
            get { return EntityTypeIds.CategoryGroup; }
        }

        public override string Description
        {
            get { return "CategoryGroup"; }
        }
    }
}