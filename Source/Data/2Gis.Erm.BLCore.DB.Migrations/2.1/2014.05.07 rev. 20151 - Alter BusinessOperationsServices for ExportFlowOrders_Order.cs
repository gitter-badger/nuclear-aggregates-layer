using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(20151, "Изменение mapping BusnessOperationServices для экспорта Invoice, выставление Descriptor = 0, т.к. CopyOrderIdentity - noncoupled", "d.ivanov")]
    public class Migration20151 : TransactedMigration
    {
        private const string CommandText = @"
UPDATE [Shared].[BusinessOperationServices]
SET [Descriptor] = 0
-- <NonCoupled>, CopyIdentity, IntegrationService.ExportFlowOrdersInvoice
WHERE [Descriptor] = -2081716307 AND [Operation] = 15113 AND [Service] = 14";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(CommandText);
        }
    }
}
