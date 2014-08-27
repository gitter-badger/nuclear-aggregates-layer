using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(22791, "Выставляем статусы одобрено всем эрм, не требующим выверки", "y.baranihin")]
    public class Migration22791 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"UPDATE aes
                                                    SET aes.Status = 1
                                                 FROM [Billing].AdvertisementElementStatuses aes
                                                    JOIN [Billing].AdvertisementElements ae on aes.Id = ae.Id
                                                    JOIN Billing.AdvertisementElementTemplates aet on ae.AdvertisementElementTemplateId = aet.Id
                                                 WHERE aes.Status != 1 and aet.NeedsValidation = 0");
        }
    }
}