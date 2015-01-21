using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAfterSaleServiceActivity : EntityTypeBase<EntityTypeAfterSaleServiceActivity>
    {
        public override int Id
        {
            get { return EntityTypeIds.AfterSaleServiceActivity; }
        }

        public override string Description
        {
            get { return "AfterSaleServiceActivity"; }
        }
    }
}