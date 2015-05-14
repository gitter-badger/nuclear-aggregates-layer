using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeChargesHistory : EntityTypeBase<EntityTypeChargesHistory>
    {
        public override int Id
        {
            get { return EntityTypeIds.ChargesHistory; }
        }

        public override string Description
        {
            get { return "ChargesHistory"; }
        }
    }
}