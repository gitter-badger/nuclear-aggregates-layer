using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeFirmDeal : EntityTypeBase<EntityTypeFirmDeal>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmDeal; }
        }

        public override string Description
        {
            get { return "FirmDeal"; }
        }
    }
}