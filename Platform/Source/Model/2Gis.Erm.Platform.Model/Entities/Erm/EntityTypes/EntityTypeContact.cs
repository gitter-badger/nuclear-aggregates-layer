using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeContact : EntityTypeBase<EntityTypeContact>
    {
        public override int Id
        {
            get { return EntityTypeIds.Contact; }
        }

        public override string Description
        {
            get { return "Contact"; }
        }
    }
}