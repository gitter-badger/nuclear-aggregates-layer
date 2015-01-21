using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCategoryGroupMembership : EntityTypeBase<EntityTypeCategoryGroupMembership>
    {
        public override int Id
        {
            get { return EntityTypeIds.CategoryGroupMembership; }
        }

        public override string Description
        {
            get { return "CategoryGroupMembership"; }
        }
    }
}