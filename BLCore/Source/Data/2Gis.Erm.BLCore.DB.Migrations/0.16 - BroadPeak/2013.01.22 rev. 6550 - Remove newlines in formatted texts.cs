using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6550, "Очищаем форматированные РМ от знаков переноса строки")]
    public sealed class Migration6550 : TransactedMigration
    {
        const string SqlCommand =
@"UPDATE AE
SET AE.Text = REPLACE(REPLACE(AE.Text, CHAR(13), ''), CHAR(10), '') , ae.ModifiedOn = getutcdate()
FROM Billing.AdvertisementElements AE
INNER JOIN Billing.AdvertisementElementTemplates AET ON AET.Id = AE.AdvertisementElementTemplateId
WHERE
AET.FormattedText = 1";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(SqlCommand);
        }
    }
}