using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeClientLink : EntityTypeBase<EntityTypeClientLink>
    {
        public override int Id
        {
            get { return EntityTypeIds.ClientLink; }
        }

        public override string Description
        {
            get { return "ClientLink"; }
        }
    }
}