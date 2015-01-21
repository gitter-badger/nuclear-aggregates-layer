using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeReleaseInfo : EntityTypeBase<EntityTypeReleaseInfo>
    {
        public override int Id
        {
            get { return EntityTypeIds.ReleaseInfo; }
        }

        public override string Description
        {
            get { return "ReleaseInfo"; }
        }
    }
}