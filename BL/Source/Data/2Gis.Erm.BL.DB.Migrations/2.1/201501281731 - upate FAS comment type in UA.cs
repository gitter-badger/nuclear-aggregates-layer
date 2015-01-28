using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations
{
    [Migration(201501281731, "Миграция существующих украинских ФАС на тип 'Новый комментарий'", "a.rechkalov")]
    public class Migration201501281731 : TransactedMigration
    {
        // Кроме как в украинских ЭРМ не должно быть украинских комментариях - поэтому не заморачиваемся
        private const string UpdateStatement = "update Billing.AdvertisementElements set FasCommentType = 6 where FasCommentType in (401, 402, 403)";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(UpdateStatement);
        }
    }
}