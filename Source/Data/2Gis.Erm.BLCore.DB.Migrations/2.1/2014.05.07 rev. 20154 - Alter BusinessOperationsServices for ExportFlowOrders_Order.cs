using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20154, "Изменение mapping BusnessOperationServices для экспорта Order, выставление Descriptor = 0, т.к. CopyOrderIdentity - noncoupled", "d.ivanov")]
    public class Migration20154 : TransactedMigration
    {
        private const string CommandText = @"
UPDATE [Shared].[BusinessOperationServices]
SET [Descriptor] = 0
-- <NonCoupled>, CopyIdentity, IntegrationService.ExportFlowOrdersOrder
WHERE [Descriptor] = -2081716307 AND [Operation] = 15113 AND [Service] = 5";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(CommandText);
        }
    }
}
