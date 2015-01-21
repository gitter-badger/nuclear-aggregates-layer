using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAdvertisementTemplate : EntityTypeBase<EntityTypeAdvertisementTemplate>
    {
        public override int Id
        {
            get { return EntityTypeIds.AdvertisementTemplate; }
        }

        public override string Description
        {
            get { return "AdvertisementTemplate"; }
        }
    }
}