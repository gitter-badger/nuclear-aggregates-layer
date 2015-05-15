using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeThemeOrganizationUnit : EntityTypeBase<EntityTypeThemeOrganizationUnit>
    {
        public override int Id
        {
            get { return EntityTypeIds.ThemeOrganizationUnit; }
        }

        public override string Description
        {
            get { return "ThemeOrganizationUnit"; }
        }
    }
}