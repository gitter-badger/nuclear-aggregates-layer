using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeThemeTemplate : EntityTypeBase<EntityTypeThemeTemplate>
    {
        public override int Id
        {
            get { return EntityTypeIds.ThemeTemplate; }
        }

        public override string Description
        {
            get { return "ThemeTemplate"; }
        }
    }
}