using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7266, "Изменение хранимок для очистки логов, создание индекса для таблицы dbo.AsyncOperationBase")]
    public sealed class Migration7266 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var alterClenupErm = ReplicationHelper.GetAttachedResource(this, "Migration7266CleanupERM");
            string ermLoggingDB = string.Format("[{0}]", context.LoggingDatabaseName);
            alterClenupErm = alterClenupErm.Replace("[ErmLogging]", ermLoggingDB);
            context.Connection.ExecuteNonQuery(alterClenupErm);

            if (context.CrmDatabaseName != null)
            {
                var alterCleanupMSCRM = ReplicationHelper.GetAttachedResource(this, "Migration7266CleanupMSCRM");
                string msCRMDB = string.Format("[{0}]", context.CrmDatabaseName);
                alterCleanupMSCRM = alterCleanupMSCRM.Replace("[DoubleGis_MSCRM]", msCRMDB);
                context.Connection.ExecuteNonQuery(alterCleanupMSCRM);
            }
        }
    }
}
