using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeFile : EntityTypeBase<EntityTypeFile>
    {
        public override int Id
        {
            get { return EntityTypeIds.File; }
        }

        public override string Description
        {
            get { return "File"; }
        }
    }
}