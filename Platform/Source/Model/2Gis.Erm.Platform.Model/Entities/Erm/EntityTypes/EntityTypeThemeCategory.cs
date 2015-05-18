using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeThemeCategory : EntityTypeBase<EntityTypeThemeCategory>
    {
        public override int Id
        {
            get { return EntityTypeIds.ThemeCategory; }
        }

        public override string Description
        {
            get { return "ThemeCategory"; }
        }
    }
}