using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(10742, "Добавляем обработку операции актуализации сервисом выгрузки заказов")]
    public sealed class Migration10742 : TransactedMigration
    {
        public const int OrderDescriptor = -2081716307;
        public const int ExportFlowOrdersOrderService = 5;
        public const int RepairOutdatedOperation = 108;

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                string.Format(
                    "IF NOT EXISTS (SELECT * FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2}) INSERT INTO [Shared].[BusinessOperationServices]( [Descriptor], [Operation], [Service]) VALUES({0}, {1}, {2})",
                    OrderDescriptor,
                    RepairOutdatedOperation,
                    ExportFlowOrdersOrderService));
        }
    }
}
