using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8782, "Изменения в схеме хранения действий")]
    public sealed class Migration8782 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var currentTimeout = context.Connection.StatementTimeout;
            context.Connection.StatementTimeout = 1200;

            var activityInstancesTable = context.Database.GetTable(new SchemaQualifiedObjectName(ErmSchemas.Activity, "Activities"));
            if (activityInstancesTable != null)
            {
                const string DealIdColumnName = "DealId";
                activityInstancesTable.RemoveForeignKey(DealIdColumnName, ErmTableNames.Deals, "Id");
                activityInstancesTable.RemoveField(DealIdColumnName);

                activityInstancesTable.Rename(ErmTableNames.ActivityInstances.Name);

                activityInstancesTable.Alter();
            }

            var activityPropertyInstancesTable = context.Database.GetTable(new SchemaQualifiedObjectName(ErmSchemas.Activity, "ActivityExtensions"));
            if (activityPropertyInstancesTable != null)
            {
                activityPropertyInstancesTable.Rename(ErmTableNames.ActivityPropertyInstances.Name);
                activityPropertyInstancesTable.Alter();
            }

            context.Connection.StatementTimeout = currentTimeout;
        }
    }
}
