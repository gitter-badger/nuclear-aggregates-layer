using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeHotClientRequest : EntityTypeBase<EntityTypeHotClientRequest>
    {
        public override int Id
        {
            get { return EntityTypeIds.HotClientRequest; }
        }

        public override string Description
        {
            get { return "HotClientRequest"; }
        }
    }
}