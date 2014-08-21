using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(23000, "Выставляем статус заглушкам - Валиден", "y.baranikhin")]
    public class Migration23000 : TransactedMigration
    {
        private const int ValidStateCode = 1;
        private const string UpdateStatementTemplate = @"update aes set Status = {0}
  FROM [Billing].[AdvertisementElements] ae join 
  [Billing].[Advertisements] a on ae.AdvertisementId = a.id join
  [Billing].[AdvertisementTemplates] at on a.AdvertisementTemplateId = at.id and a.Id = at.DummyAdvertisementId join
  [Billing].[AdvertisementElementStatuses] aes on ae.Id = aes.id
  where aes.Status != {0}";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(UpdateStatementTemplate, ValidStateCode));
        }
    }
}