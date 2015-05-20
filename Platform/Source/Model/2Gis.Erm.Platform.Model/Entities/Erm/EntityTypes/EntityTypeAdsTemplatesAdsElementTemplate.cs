using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAdsTemplatesAdsElementTemplate : EntityTypeBase<EntityTypeAdsTemplatesAdsElementTemplate>
    {
        public override int Id
        {
            get { return EntityTypeIds.AdsTemplatesAdsElementTemplate; }
        }

        public override string Description
        {
            get { return "AdsTemplatesAdsElementTemplate"; }
        }
    }
}