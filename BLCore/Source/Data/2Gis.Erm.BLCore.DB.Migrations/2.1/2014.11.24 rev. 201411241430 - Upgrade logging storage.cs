using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201411241430, "Обновление хранилища для логов", "i.maslennikov")]
    public sealed class Migration201411241430 : TransactedMigration, INonDefaultDatabaseMigration
    {
        public ErmConnectionStringKey ConnectionStringKey 
        {
            get { return ErmConnectionStringKey.Logging; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources._201411241430_UpgradeLoggingStorage_);
        }
    }
}