using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeNote : EntityTypeBase<EntityTypeNote>
    {
        public override int Id
        {
            get { return EntityTypeIds.Note; }
        }

        public override string Description
        {
            get { return "Note"; }
        }
    }
}