using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAdvertisementElementTemplate : EntityTypeBase<EntityTypeAdvertisementElementTemplate>
    {
        public override int Id
        {
            get { return EntityTypeIds.AdvertisementElementTemplate; }
        }

        public override string Description
        {
            get { return "AdvertisementElementTemplate"; }
        }
    }
}