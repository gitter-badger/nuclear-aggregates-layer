using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeTheme : EntityTypeBase<EntityTypeTheme>
    {
        public override int Id
        {
            get { return EntityTypeIds.Theme; }
        }

        public override string Description
        {
            get { return "Theme"; }
        }
    }
}