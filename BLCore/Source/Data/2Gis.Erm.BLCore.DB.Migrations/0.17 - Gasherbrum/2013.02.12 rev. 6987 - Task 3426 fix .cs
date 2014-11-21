using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6987, @"Вычищаем из таблицы AdvertisementElements символ \r")]
    public sealed class Migration6987 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"
            UPDATE Billing.AdvertisementElements
            SET Text = REPLACE(Text, CHAR(13), '')
            ");
        }
    }
}
