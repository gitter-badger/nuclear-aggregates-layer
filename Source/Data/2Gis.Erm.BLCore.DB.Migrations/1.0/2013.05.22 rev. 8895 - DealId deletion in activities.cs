using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8895, "Удаление DealId действиях. Еще раз")]
    public sealed class Migration8895 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var currentTimeout = context.Connection.StatementTimeout;
            context.Connection.StatementTimeout = 1200;

            var activityInstancesTable = context.Database.GetTable(ErmTableNames.ActivityInstances);
            if (activityInstancesTable != null)
            {
                const string DealIdColumnName = "DealId";
                activityInstancesTable.RemoveForeignKey(DealIdColumnName, ErmTableNames.Deals, "Id");
                activityInstancesTable.RemoveField(DealIdColumnName);

                activityInstancesTable.Alter();
            }

            context.Connection.StatementTimeout = currentTimeout;
        }
    }
}
