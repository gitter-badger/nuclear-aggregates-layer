using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeLimit : EntityTypeBase<EntityTypeLimit>
    {
        public override int Id
        {
            get { return EntityTypeIds.Limit; }
        }

        public override string Description
        {
            get { return "Limit"; }
        }
    }
}