using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(20146, "Изменение mapping BusnessOperationServices поддержки выгрузки Invocies а поток floworders в соответствии с ERM-3964", "d.ivanov")]
    public class Migration20146 : TransactedMigration
    {
        private const string CommandText = @"
DELETE FROM [Shared].[BusinessOperationServices]
WHERE Service = 14

INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service])
VALUES 
    -- EntityName.Order, AssignIdentity, IntegrationService.ExportFlowOrdersInvoice
    (-2081716307, 4, 14),

    -- EntityName.Order, CopyOrderIdentity, IntegrationService.ExportFlowOrdersInvoice
    (-2081716307, 15113, 14),

    -- EntityName.Order, CreateIdentity, IntegrationService.ExportFlowOrdersInvoice
    (-2081716307, 30, 14),

    -- EntityName.Order, UpdateIdentity, IntegrationService.ExportFlowOrdersInvoice
    (-2081716307, 31, 14),

    -- <NonCoupled>, WithdrawalIdentity, IntegrationService.ExportFlowOrdersInvoice
    (0, 32, 14),

    -- <NonCoupled>, RevertWithdrawalIdentity, IntegrationService.ExportFlowOrdersInvoice
    (0, 33, 14),

    -- <NonCoupled>, ImportFirmIdentity, IntegrationService.ExportFlowOrdersInvoice
    (0, 14602, 14),

    -- <NonCoupled>, RepairOutdatedIdentity, IntegrationService.ExportFlowOrdersInvoice
    (0, 15107, 14),

    -- <NonCoupled>, CloseWithDenialIdentity, IntegrationService.ExportFlowOrdersInvoice
    (0, 15108, 14),

    -- EntityName.LegalPerson, MergeIdentity, IntegrationService.ExportFlowOrdersInvoice
    (942141479, 25, 14),

    -- EntityName.OrderPosition, DeleteIdentity, IntegrationService.ExportFlowOrdersInvoice
    (1905845000, 9, 14),

    -- EntityName.OrderPosition, CreateIdentity, IntegrationService.ExportFlowOrdersInvoice
    (1905845000, 30, 14),

    -- EntityName.OrderPosition, UpdateIdentity, IntegrationService.ExportFlowOrdersInvoice
    (1905845000, 31, 14)
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(CommandText);
        }
    }
}
