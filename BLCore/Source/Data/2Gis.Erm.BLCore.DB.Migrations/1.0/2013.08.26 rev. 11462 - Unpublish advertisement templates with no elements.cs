using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11462, "Делаем неопубликованными шаблоны РМ без элементов шаблона РМ")]
    public sealed class Migration11462 : TransactedMigration
    {
        public const string UpdateAdvertisementTerritories = @"
update Billing.AdvertisementTemplates set IsPublished = 0
from (Billing.AdvertisementTemplates at
	  left join Billing.AdsTemplatesAdsElementTemplates ataet on ataet.AdsTemplateId = at.Id)
where ataet.Id is null and at.IsDeleted = 0 and at.IsPublished = 1";
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(UpdateAdvertisementTerritories);
        }
    }
}