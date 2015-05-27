using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAdvertisementElement : EntityTypeBase<EntityTypeAdvertisementElement>
    {
        public override int Id
        {
            get { return EntityTypeIds.AdvertisementElement; }
        }

        public override string Description
        {
            get { return "AdvertisementElement"; }
        }
    }
}