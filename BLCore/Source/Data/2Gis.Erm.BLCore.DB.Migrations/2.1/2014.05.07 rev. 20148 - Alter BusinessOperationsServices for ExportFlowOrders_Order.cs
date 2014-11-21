using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(20148, "Изменение mapping BusnessOperationServices для экспорта Order, замена CopyIdentity на CopyOrderIdentity", "d.ivanov")]
    public class Migration20148 : TransactedMigration
    {
        private const string CommandText = @"
UPDATE [Shared].[BusinessOperationServices]
SET [Operation] = 15113
-- EntityName.Order, CopyIdentity, IntegrationService.ExportFlowOrdersOrder
WHERE [Descriptor] = -2081716307 AND [Operation] = 24 AND [Service] = 5";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(CommandText);
        }
    }
}
