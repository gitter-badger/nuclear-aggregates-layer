using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeReference : EntityTypeBase<EntityTypeReference>
    {
        public override int Id
        {
            get { return EntityTypeIds.Reference; }
        }

        public override string Description
        {
            get { return "Reference"; }
        }
    }
}