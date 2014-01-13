using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6766, "Создание хранимок для очистки логов.")]
    public sealed class Migration6766 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var createCleanupERM = ReplicationHelper.GetAttachedResource(this, "Migration6766CreateCleanupERM");
            string ermLoggingDB = string.Format("[{0}]", context.LoggingDatabaseName);
            createCleanupERM = createCleanupERM.Replace("[ErmLogging]", ermLoggingDB);
            context.Connection.ExecuteNonQuery(createCleanupERM);

            if (context.CrmDatabaseName != null)
            {
                var createCleanupMSCRM = ReplicationHelper.GetAttachedResource(this, "Migration6766CreateCleanupMSCRM");
                string msCRMDB = string.Format("[{0}]", context.CrmDatabaseName);
                createCleanupMSCRM = createCleanupMSCRM.Replace("[DoubleGis_MSCRM]", msCRMDB);
                context.Connection.ExecuteNonQuery(createCleanupMSCRM);
            }
        }
    }
}
