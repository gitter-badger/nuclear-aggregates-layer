using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeFirm : EntityTypeBase<EntityTypeFirm>
    {
        public override int Id
        {
            get { return EntityTypeIds.Firm; }
        }

        public override string Description
        {
            get { return "Firm"; }
        }
    }
}