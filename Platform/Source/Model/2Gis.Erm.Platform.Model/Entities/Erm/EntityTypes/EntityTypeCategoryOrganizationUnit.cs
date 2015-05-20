using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCategoryOrganizationUnit : EntityTypeBase<EntityTypeCategoryOrganizationUnit>
    {
        public override int Id
        {
            get { return EntityTypeIds.CategoryOrganizationUnit; }
        }

        public override string Description
        {
            get { return "CategoryOrganizationUnit"; }
        }
    }
}