using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201411241430, "Обновление хранилища для логов", "i.maslennikov")]
    public class Migration201411241430 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var resultSql = string.Format(Resources._201411241430_UpgradeLoggingStorage_, context.LoggingDatabaseName);
            context.Database.ExecuteNonQuery(resultSql);
        }
    }
}